using System.Collections.Generic;
using Game.Block.Blocks;
using Game.Block.Blocks.BeltConveyor;
using Game.Block.Component;
using Game.Block.Component.IOConnector;
using Game.Block.Config.LoadConfig.Param;
using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.Block.Interface.Component;
using Game.Context;
using UnityEngine;

namespace Game.Block.Factory.BlockTemplate
{
    public class VanillaBeltConveyorTemplate : IBlockTemplate
    {
        public IBlock New(BlockConfigData param, int entityId, long blockHash, BlockPositionInfo blockPositionInfo)
        {
            var beltParam = param.Param as BeltConveyorConfigParam;
            var blockName = ServerContext.BlockConfig.GetBlockConfig(blockHash).Name;

            var connectorComponent = CreateConnector(blockPositionInfo, blockHash);
            var beltComponent = new VanillaBeltConveyorComponent(beltParam.BeltConveyorItemNum, beltParam.TimeOfItemEnterToExit, connectorComponent, blockName);
            var components = new List<IBlockComponent>
            {
                beltComponent,
                connectorComponent,
            };

            return new BlockSystem(entityId, param.BlockId, components, blockPositionInfo);
        }

        public IBlock Load(BlockConfigData param, int entityId, long blockHash, string state, BlockPositionInfo blockPositionInfo)
        {
            var beltParam = param.Param as BeltConveyorConfigParam;
            var blockName = ServerContext.BlockConfig.GetBlockConfig(blockHash).Name;

            var connectorComponent = CreateConnector(blockPositionInfo, blockHash);
            var beltComponent = new VanillaBeltConveyorComponent(state, beltParam.BeltConveyorItemNum, beltParam.TimeOfItemEnterToExit, connectorComponent,blockName);
            var components = new List<IBlockComponent>
            {
                beltComponent,
                connectorComponent,
            };

            return new BlockSystem(entityId, param.BlockId, components, blockPositionInfo);
        }

        public const string SlopeUpBeltConveyor = "gear belt conveyor up";
        public const string SlopeDownBeltConveyor = "gear belt conveyor down";
        public const string Hueru = "gear belt conveyor hueru";
        public const string Kieru = "gear belt conveyor kieru";


        private BlockConnectorComponent<IBlockInventory> CreateConnector(BlockPositionInfo blockPositionInfo, long blockHash)
        {
            var config = ServerContext.BlockConfig.GetBlockConfig(blockHash);
            if (config.Name == SlopeUpBeltConveyor)
            {
                Debug.Log("SlopeUpBeltConveyor");
                return new BlockConnectorComponent<IBlockInventory>(new IOConnectionSetting(
                    // 南のみ接続を受け、アイテムをインプットする
                    new ConnectDirection[] { new(-1, 0, 0), new(-1, 0, -1) },
                    //上の北向きに出力する
                    new ConnectDirection[] { new(1, 0, 1) },
                    new[]
                    {
                        VanillaBlockType.Machine, VanillaBlockType.Chest, VanillaBlockType.Generator,
                        VanillaBlockType.Miner, VanillaBlockType.BeltConveyor,
                    }), blockPositionInfo);
            }
            if (config.Name == SlopeDownBeltConveyor)
            {
                Debug.Log("SlopeDownBeltConveyor");
                return new BlockConnectorComponent<IBlockInventory>(new IOConnectionSetting(
                    // 上南のみ接続を受け、アイテムをインプットする
                    new ConnectDirection[] { new(-1, 0, 1) },
                    //北向き、もしくはひとつ下のダウンベルコンに出力する
                    new ConnectDirection[] { new(1, 0, 0), new(1, 0, -1) },
                    new[]
                    {
                        VanillaBlockType.Machine, VanillaBlockType.Chest, VanillaBlockType.Generator,
                        VanillaBlockType.Miner, VanillaBlockType.BeltConveyor,
                    }), blockPositionInfo);
            }

            //TODo UP bletからの入力を受付

            return new BlockConnectorComponent<IBlockInventory>(new IOConnectionSetting(
                // 南、西、東をからの接続を受け、アイテムをインプットする
                new ConnectDirection[]
                {
                    new(-1, 0, 0), // 後ろ側
                    new(-1, 0, -1), // 後ろ上からの入力（下り坂）
                    new(-1, 0, 1), // 後ろ下からの入力（上り坂）
                    new(0, 1, 0), new(0, -1, 0)
                },
                //北向きに出力する
                new ConnectDirection[]
                {
                    new(1, 0, 0),
                    new(1, 0, 1),
                    new(1, 0, -1),
                },
                new[]
                {
                    VanillaBlockType.Machine, VanillaBlockType.Chest, VanillaBlockType.Generator,
                    VanillaBlockType.Miner, VanillaBlockType.BeltConveyor,
                }), blockPositionInfo);
        }
    }
}