using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Train.RailGraph
{
    public class RailGraphDatastore
    {
        private static RailGraphDatastore _instance;
        private readonly Dictionary<RailNode, int> railIdDic;//RailNode→Id辞書。下の逆引き
        private readonly List<RailNode> railNodes;//Id→RailNode辞書。上の逆引き
        MinHeap<int> nextidQueue;//上のリストで穴開き状態をなるべく防ぐために、次使う最小Idを取得するためのキュー。そのためだけにminheapを実装している

        //以下は経路探索で使う変数
        private readonly List<List<(int, int)>> connectNodes;//ノード接続情報。connectNodes[NodeId_A]:NodeId_Aのつながる先のNodeIdリスト。(Id,距離)

        public RailGraphDatastore()
        {
            _instance = this;
            railIdDic = new Dictionary<RailNode, int>();
            railNodes = new List<RailNode>();
            nextidQueue = new MinHeap<int>();
            connectNodes = new List<List<(int, int)>>();
        }

        //このクラスはシングルトンである
        //gearを参考にinternal化
        public static void AddNode(RailNode node)
        {
            _instance.AddNodeInternal(node); 
        }
        public static void ConnectNode(RailNode node, RailNode targetNode, int distance) 
        {
            _instance.ConnectNodeInternal(node, targetNode, distance); 
        }
        public static void DisconnectNode(RailNode node, RailNode targetNode)
        {
            _instance.DisconnectNodeInternal(node, targetNode);
        }
        public static List<(RailNode, int)> GetConnectedNodesWithDistance(RailNode node)
        {
            return _instance.GetConnectedNodesWithDistanceInternal(node);
        }
        public static void RemoveNode(RailNode node) 
        {
            _instance.RemoveNodeInternal(node);
        }
        public static int GetDistanceBetweenNodes(RailNode start, RailNode target)
        {
            return _instance.GetDistanceBetweenNodesInternal(start, target);
        }




        private void AddNodeInternal(RailNode node)
        {
            //すでにnodeが登録されている場合は何もしない
            if (railIdDic.ContainsKey(node))
                return;

            int nextid;
            if ((nextidQueue.IsEmpty) || (railNodes.Count < nextidQueue.Peek()))
                nextidQueue.Insert(railNodes.Count);
            nextid = nextidQueue.RemoveMin();//より小さいIdを使いたい
            //この時点でnextid<=railNodes.Countは確定
            if (nextid == railNodes.Count)
            {
                railNodes.Add(node);
                connectNodes.Add(new List<(int, int)>());
            }
            else
            {
                railNodes[nextid] = node;
            }
            railIdDic[node] = nextid;
        }

        //接続元RailNode、接続先RailNode、int距離
        private void ConnectNodeInternal(RailNode node, RailNode targetNode, int distance)
        {
            //nodeが辞書になければ追加
            if (!railIdDic.ContainsKey(node))
                AddNode(node);
            var nodeid = railIdDic[node];
            //targetが辞書になければ追加
            if (!railIdDic.ContainsKey(targetNode))
                AddNode(targetNode);
            var targetid = railIdDic[targetNode];
            //connectNodes[nodeid]にtargetidがなければ追加
            if (!connectNodes[nodeid].Any(x => x.Item1 == targetid))
                connectNodes[nodeid].Add((targetid, distance));
        }
        //接続削除
        private void DisconnectNodeInternal(RailNode node, RailNode targetNode)
        {
            var nodeid = railIdDic[node];
            var targetid = railIdDic[targetNode];
            connectNodes[nodeid].RemoveAll(x => x.Item1 == targetid);
        }

        //ノードの削除。削除対象のノードに向かう経路の削除は別に行う必要がある
        //"削除対象のノードに向かう経路"の情報はこのクラスでしか管理してないので、このクラスで削除する
        private void RemoveNodeInternal(RailNode node)
        {
            //railIdDicにnodeがなければ何もしない
            if (!railIdDic.ContainsKey(node))
                return;
            var nodeid = railIdDic[node];
            railIdDic.Remove(node);
            railNodes[nodeid] = null;
            nextidQueue.Insert(nodeid);
            connectNodes[nodeid].Clear();
            RemoveNodeTo(nodeid);//削除対象のノードに向かう経路の削除
        }

        //削除対象のノードに向かう経路の削除
        //これはnode反転したときのすべての行き先から見ればいい。ただし反転nodeがすでに存在しないと思わぬバグになるしそこまで考慮するとコードが複雑になるので全探索する
        private void RemoveNodeTo(int nodeid)
        {
            for (int i = 0; i < connectNodes.Count; i++)
            {
                connectNodes[i].RemoveAll(x => x.Item1 == nodeid);
            }
        }


        //RailNodeの入力に対しつながっているRailNodeをリスト<Node,距離int>で返す
        private List<(RailNode, int)> GetConnectedNodesWithDistanceInternal(RailNode node)
        {
            if (!railIdDic.ContainsKey(node))
                return new List<(RailNode, int)>();
            int nodeId = railIdDic[node];
            return connectNodes[nodeId].Select(x => (railNodes[x.Item1], x.Item2)).ToList();
        }


        //start-targetは直接つながってないとダメなことに注意
        //railnode2つの入力 start から target までの距離を返す
        //つながっていない場合は-1を返して警告だす
        private int GetDistanceBetweenNodesInternal(RailNode start, RailNode target)
        {
            if (!railIdDic.ContainsKey(start) || !railIdDic.ContainsKey(target)) 
            {
                Debug.LogWarning("RailNodeが登録されていません");
                return -1;
            }
            int startid = railIdDic[start];
            int targetid = railIdDic[target];
            foreach (var (neighbor, distance) in connectNodes[startid])
            {
                if (neighbor == targetid)
                    return distance;
            }
            //警告だす
            Debug.LogWarning("RailNodeがつながっていません" + startid + "to" + targetid + "");
            return -1;
        }


        /// <summary>
        /// ダイクストラ法を用いて開始ノードから目的地ノードまでの最短経路を計算します。
        /// この高速化のためにノードをintのIDで管理しています。
        /// generated by chat gpt 4o
        /// <returns>最短経路のノード順リスト</returns>
        public List<RailNode> FindShortestPath(RailNode startNode, RailNode targetNode)
        {
            return FindShortestPath(railIdDic[startNode], railIdDic[targetNode]);
        }

        public List<RailNode> FindShortestPath(int startid, int targetid)
        {
            // 優先度付きキュー（距離が小さい順）
            var priorityQueue = new PriorityQueue<int, int>();
            // 各ノードへの最短距離を記録する（初期値は無限大を表す int.MaxValue）
            List<int> distances = new List<int>();// 各ノードへの最短距離を記録する（初期値は無限大を表す int.MaxValue）
            List<int> previousNodes = new List<int>();// 各ノードの前に訪れたノード
            for (int i = 0; i < railNodes.Count; i++)
                distances.Add(int.MaxValue);
            for (int i = 0; i < railNodes.Count; i++)
                previousNodes.Add(-1);


            // 開始ノードの距離を0に設定し、優先度付きキューに追加
            distances[startid] = 0;
            priorityQueue.Enqueue(startid, 0);

            while (priorityQueue.Count > 0)
            {
                // 現在のノードを取得
                var currentNodecnt = priorityQueue.Dequeue();
                // 目的地に到達したら終了
                if (currentNodecnt == targetid)
                {
                    break;
                }

                // 現在のノードからつながる全てのノードを確認
                foreach (var (neighbor, distance) in connectNodes[currentNodecnt])
                {
                    int newDistance = distances[currentNodecnt] + distance;
                    // なにかの間違いでintがオーバーフローした場合(経路が長すぎたり)
                    if (newDistance < 0)
                        continue;
                    // より短い距離が見つかった場合
                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        previousNodes[neighbor] = currentNodecnt;
                        // キューに隣接ノードを追加または更新
                        priorityQueue.Enqueue(neighbor, newDistance);
                    }
                }
            }

            // 経路を逆順でたどる
            var path = new List<int>();
            var current = targetid;
            while (current != -1)
            {
                path.Add(current);
                current = previousNodes[current];
            }

            // 開始ノードまでたどり着けなかった場合は空のリストを返す
            if (path.Last() != startid)
            {
                return new List<RailNode>();
            }

            // 経路を正しい順序に並べて返す
            path.Reverse();
            var pathNodes = path.Select(id => railNodes[id]).ToList();
            return pathNodes;
        }











    }
}
