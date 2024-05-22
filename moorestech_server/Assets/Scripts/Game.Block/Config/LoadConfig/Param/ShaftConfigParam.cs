using System.Collections.Generic;
using Core.Item.Interface.Config;
using Game.Block.Config.LoadConfig.OptionLoader;
using Game.Block.Interface.BlockConfig;

namespace Game.Block.Config.LoadConfig.Param
{
    public class ShaftConfigParam : IBlockConfigParam
    {
        public readonly float LossPower;
        public List<ConnectSettings> GearConnectSettings;
        
        private ShaftConfigParam(dynamic blockParam, IItemConfig itemConfig)
        {
            LossPower = blockParam.lossPower;
            GearConnectSettings = BlockConfigJsonLoad.GetConnectSettings(blockParam, "gearConnects", GearConnectOptionLoader.Loader);
        }
        
        public static IBlockConfigParam Generate(dynamic blockParam, IItemConfig itemConfig)
        {
            return new ShaftConfigParam(blockParam, itemConfig);
        }
    }
}