using System;
using Game.Block.Blocks.Gear;
using Game.Block.Config.LoadConfig.Param;
using Game.Block.Interface;
using Game.Context;
using Game.Gear.Common;
using Game.World.Interface.Util;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Boot;
using Tests.Module.TestMod;
using UnityEngine;

namespace Tests.CombinedTest.Game
{
    public class GearNetworkTest
    {
        [Test]
        //シンプルジェネレーターを設置し、簡易的な歯車NWを作り、RPM、回転方向があっているかをテスト
        //Install a simple generator, make a simple gear NW, and test if RPM and direction of rotation are correct.
        public void SimpleGeneratorAndGearRpmTest()
        {
            var (_, serviceProvider) = new MoorestechServerDIContainerGenerator().Create(TestModDirectory.ForUnitTestModDirectory);

            var generator = AddBlock(ForUnitTestModBlockId.SimpleGearGenerator, Vector3Int.zero, BlockDirection.North);
            var shaft = AddBlock(ForUnitTestModBlockId.Shaft, new Vector3Int(0,0,1), BlockDirection.North);
            var bigGear = AddBlock(ForUnitTestModBlockId.BigGear, new Vector3Int(0,0,2), BlockDirection.North);
            var smallGear = AddBlock(ForUnitTestModBlockId.SmallGear, new Vector3Int(2,0,2), BlockDirection.North);
            
            //ネットワークをアップデート
            //Update the network
            var gearNetworkDatastore = serviceProvider.GetService<GearNetworkDatastore>();
            var gearNetwork = gearNetworkDatastore.GearNetworks[0];
            gearNetwork.ManualUpdate();
            
            //ジェネレーターの供給が正しいか
            //Is the generator supply correct?
            var generatorComponent = generator.ComponentManager.GetComponent<GearGeneratorComponent>();
            Assert.AreEqual(10.0f, generatorComponent.CurrentRpm);
            Assert.AreEqual(true, generatorComponent.GenerateIsClockwise);
            
            //シャフトの回転は正しいか
            //Is the rotation of the shaft correct?
            var shaftComponent = shaft.ComponentManager.GetComponent<GearComponent>();
            Assert.AreEqual(10.0f, shaftComponent.CurrentRpm);
            Assert.AreEqual(true, shaftComponent.IsCurrentClockwise);
            
            //BigGearの回転は正しいか
            //Is the rotation of BigGear correct?
            var bigGearComponent = bigGear.ComponentManager.GetComponent<GearComponent>();
            Assert.AreEqual(10.0f, bigGearComponent.CurrentRpm);
            Assert.AreEqual(true, bigGearComponent.IsCurrentClockwise);
            
            //SmallGearの回転は正しいか
            //Is the rotation of SmallGear correct?
            var smallGearComponent = smallGear.ComponentManager.GetComponent<GearComponent>();
            Assert.AreEqual(20.0f, smallGearComponent.CurrentRpm); // ギア比2:1 Gear ratio 2:1
            Assert.AreEqual(false, smallGearComponent.IsCurrentClockwise); // 回転が反転する Rotation is reversed
        }

        [Test]
        public void LoopGearNetworkTest()
        {
            //TODO ループした歯車NWを作成し、RPM、回転方向があっているかをテスト
        }

        [Test]
        public void DifferentRpmGearNetworkToRockTest()
        {
            //TODO RPMが違ってロックされるテスト
        }

        [Test]
        public void DifferentDirectionGearNetworkToRockTest()
        {
            //TODO 回転方向が違ってロックされるテスト
        }


        [Test]
        public void MultiGeneratorOverrideRpmTest()
        {
            //TODO 複数のジェネレーターのRPMがオーバーライドされるテスト
        }


        [Test]
        public void MultiGeneratorDifferentDirectionToRockTest()
        {
            //TODO 複数のジェネレーターの回転方向が違うことでロックされるテスト
        }

        [Test]
        public void ServeTorqueTest()
        {
            //TODO 機械によってトルクが消費されるテスト（正しいトルクが供給されるかのテスト
            //TODO 供給トルクが足りないときに稼働時間が長くなるテスト
        }

        [Test]
        public void ServeTorqueOverTest()
        {
            //TODO トルクが多いとその分供給トルクが減るテスト
        }

        [Test]
        public void GearNetworkMergeTest()
        {
            //TODO 設置したら歯車NWが増える、歯車NWのマージのテスト、削除したら歯車NWが分割されるテスト
        }

        private static IBlock AddBlock(int blockId,Vector3Int pos,BlockDirection direction)
        {
            var config = ServerContext.BlockConfig.GetBlockConfig(blockId);
            
            var posInfo = new BlockPositionInfo(pos, direction, config.BlockSize);
            var block = ServerContext.BlockFactory.Create(blockId, CreateBlockEntityId.Create(), posInfo);
            ServerContext.WorldBlockDatastore.AddBlock(block);
            return block;
        }
    }
}