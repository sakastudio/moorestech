using System;
using System.Collections.Generic;
using Core.ConfigJson;
using Core.Const;
using Core.Item.Config;
using Game.Block.Config.LoadConfig;
using Game.Block.Config.LoadConfig.Param;
using Game.Block.Interface.BlockConfig;
using UnityEngine;

namespace Game.Block.Config
{
    //todo クライアントのためにそのブロックタイプがopneableInventoryを持っているかをチェックするクラスを作成する
    public class BlockConfig : IBlockConfig
    {
        private readonly List<BlockConfigData> _blockConfigList;
        private readonly Dictionary<long, BlockConfigData> _bockHashToConfig = new();
        private readonly Dictionary<string, List<int>> _modIdToBlockIds = new();

        public BlockConfig(ConfigJsonList configJson, IItemConfig itemConfig)
        {
            _blockConfigList =
                new BlockConfigJsonLoad(itemConfig).LoadFromJsons(configJson.BlockConfigs, configJson.SortedModIds);
            foreach (var blockConfig in _blockConfigList)
            {
                if (_bockHashToConfig.ContainsKey(blockConfig.BlockHash))
                    throw new Exception("ブロック名 " + blockConfig.Name + " は重複しています。");

                _bockHashToConfig.Add(blockConfig.BlockHash, blockConfig);

                var blockId = blockConfig.BlockId;
                if (_modIdToBlockIds.TryGetValue(blockConfig.ModId, out var blockIds))
                    blockIds.Add(blockId);
                else
                    _modIdToBlockIds.Add(blockConfig.ModId, new List<int> { blockId });
            }
        }

        public BlockConfigData GetBlockConfig(int id)
        {
            //0は空気ブロックなので1を引いておくs
            id -= 1;
            if (id < 0) throw new ArgumentException("id must be greater than 0 ID:" + id);
            if (id < _blockConfigList.Count) return _blockConfigList[id];


            //未定義の時はNullBlockConfigを返す
            //idを元に戻す
            id++;
            //TODo ここのエラーハンドリングをどうするか決める
            return new BlockConfigData("mod is not found", id,
                "ID " + id + " is undefined",
                0,
                VanillaBlockType.Block,
                new NullBlockConfigParam(),
                ItemConst.EmptyItemId, new ModelTransform(), new Vector2Int(1, 1));
        }

        public BlockConfigData GetBlockConfig(long blockHash)
        {
            if (_bockHashToConfig.TryGetValue(blockHash, out var blockConfig)) return blockConfig;

            throw new Exception("BlockHash not found:" + blockHash);
        }

        public BlockConfigData GetBlockConfig(string modId, string blockName)
        {
            foreach (var blockConfig in _blockConfigList)
                if (blockConfig.ModId == modId && blockConfig.Name == blockName)
                    return blockConfig;
            //TODO ログ基盤に入れる
            throw new Exception("Mod id or block name not found:" + modId + " " + blockName);
        }

        public int GetBlockConfigCount()
        {
            return _blockConfigList.Count;
        }

        public List<int> GetBlockIds(string modId)
        {
            return _modIdToBlockIds.TryGetValue(modId, out var blockIds) ? blockIds : new List<int>();
        }
    }
}