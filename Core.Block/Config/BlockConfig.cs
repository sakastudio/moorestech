using System;
using System.Collections.Generic;
using Core.Block.Config.LoadConfig;
using Core.Block.Config.LoadConfig.Param;
using Core.ConfigJson;
using Core.Const;
using Core.Item.Util;

namespace Core.Block.Config
{
    public class BlockConfig : IBlockConfig
    {
        private readonly List<BlockConfigData> _blockConfigList;

        public BlockConfig(ConfigJsonList configJson)
        {
            _blockConfigList = new BlockConfigJsonLoad().LoadFromJsons(configJson.SortedBlockConfigJsonList);
        }

        public BlockConfigData GetBlockConfig(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("id must be greater than 0 ID:" + id);
            }
            if (id < _blockConfigList.Count)
            {
                return _blockConfigList[id];
            }

            //未定義の時はNullBlockConfigを返す
            return new BlockConfigData(id,
                "ID " + id + " is undefined",
                VanillaBlockType.Block,
                new NullBlockConfigParam(),
                ItemConst.EmptyItemId);
        }

        public int GetBlockConfigCount() { return _blockConfigList.Count; }
    }
}