using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame.UnityView.Chunk
{
    [CreateAssetMenu(fileName = "BlockObjects", menuName = "BlockObjects", order = 0)]
    public class BlockObjects : ScriptableObject
    {
        [SerializeField] private List<BlockGameObject> BlockObjectList;
        [SerializeField] private BlockGameObject NothingIndexBlockObject;

        public BlockGameObject GetBlock(int index)
        {
            if (BlockObjectList.Count <= index)
            {
                return NothingIndexBlockObject;
            }

            return BlockObjectList[index];
        }
    }
    
}