using System;
using System.Collections.Generic;
using Core.Const;
using Game.Block.Interface;
using Game.Block.Interface.State;
using UnityEngine;

namespace Game.World.Interface.DataStore
{
    public interface IWorldBlockDatastore
    {
        public IObservable<(ChangedBlockState state, WorldBlockData blockData)> OnBlockStateChange { get; }

        public bool AddBlock(IBlock block, Vector3Int pos, BlockDirection blockDirection);
        public bool RemoveBlock(Vector3Int pos);
        public IBlock GetBlock(Vector3Int pos);
        public WorldBlockData GetOriginPosBlock(Vector3Int pos);
        public Vector3Int GetBlockPosition(int entityId);
        public BlockDirection GetBlockDirection(Vector3Int pos);
        public List<SaveBlockData> GetSaveBlockDataList();
        public void LoadBlockDataList(List<SaveBlockData> saveBlockDataList);
    }

    public static class WorldBlockDatastoreUtil
    {
        public static bool Exists(this IWorldBlockDatastore datastore,Vector3Int pos)
        {
            return datastore.GetBlock(pos).BlockId != BlockConst.EmptyBlockId;
        }

        public static bool TryGetBlock(this IWorldBlockDatastore datastore,Vector3Int pos, out IBlock block)
        {
            block = datastore.GetBlock(pos);
            return block.EntityId != BlockConst.NullBlockEntityId;
        }
        
        public static bool ExistsComponent<TComponent>(this IWorldBlockDatastore datastore,Vector3Int pos)
        {
            return datastore.GetBlock(pos) is TComponent || //TODo これを消す
                   datastore.GetBlock(pos).ComponentManager.ExistsComponent<TComponent>();
        }

        public static TComponent GetBlock<TComponent>(this IWorldBlockDatastore datastore,Vector3Int pos)
        {
            var block = datastore.GetBlock(pos);
            
            //TODO 下を消す
            if (block is TComponent component) return component;
            
            if (block.ComponentManager.TryGetComponent(out TComponent component2)) return component2;

            return default;
        }

        public static bool TryGetBlock<TComponent>(this IWorldBlockDatastore datastore,Vector3Int pos, out TComponent component)
        {
            if (datastore.ExistsComponent<TComponent>(pos))
            {
                component = datastore.GetBlock<TComponent>(pos);
                return true;
            }

            component = default;
            return false;
        }
    }
}