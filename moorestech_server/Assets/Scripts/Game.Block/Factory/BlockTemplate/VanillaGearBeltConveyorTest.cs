using System.Collections.Generic;
using Game.Block.Blocks;
using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.Block.Interface.Component;

namespace Game.Block.Factory.BlockTemplate
{
    public class VanillaGearBeltConveyorTest : IBlockTemplate
    {
        public IBlock New(BlockConfigData config, int entityId, BlockPositionInfo blockPositionInfo)
        {
            var blockComponents = new List<IBlockComponent>();
            return new BlockSystem(entityId, config.BlockId, blockComponents, blockPositionInfo);
        }
        public IBlock Load(string state, BlockConfigData config, int entityId, BlockPositionInfo blockPositionInfo)
        {
            var blockComponents = new List<IBlockComponent>();
            return new BlockSystem(entityId, config.BlockId, blockComponents, blockPositionInfo);
        }
    }
}