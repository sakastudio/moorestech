using System.Reflection;
using Core.Inventory;
using Game.Block.Interface;
using Game.Block.Blocks.Miner;
using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.Context;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Boot;
using Tests.Module.TestMod;
using UnityEngine;

namespace Tests.UnitTest.Core.Block
{
    public class MinerSaveLoadTest
    {
        private const int MinerId = ForUnitTestModBlockId.MinerId;

        [Test]
        public void SaveLoadTest()
        {
            var (_, serviceProvider) =
                new MoorestechServerDIContainerGenerator().Create(TestModDirectory.ForUnitTestModDirectory);
            var blockFactory = ServerContext.BlockFactory;
            var minerHash = ServerContext.BlockConfig.GetBlockConfig(MinerId).BlockHash;

            var minerPosInfo = new BlockPositionInfo(new Vector3Int(0, 0), BlockDirection.North, Vector3Int.one);
            var originalMiner = blockFactory.Create(MinerId, 1,minerPosInfo);
            var originalRemainingMillSecond = 350;

            var inventory =
                (OpenableInventoryItemDataStoreService)typeof(VanillaElectricMinerComponent)
                    .GetField("_openableInventoryItemDataStoreService", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(originalMiner);
            inventory.SetItem(0, 1, 1);
            inventory.SetItem(2, 4, 1);
            typeof(VanillaElectricMinerComponent).GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(originalMiner, originalRemainingMillSecond);


            var json = originalMiner.GetSaveState();
            Debug.Log(json);


            var loadedMiner = blockFactory.Load(minerHash, 1, json,minerPosInfo);
            var loadedInventory =
                (OpenableInventoryItemDataStoreService)typeof(VanillaElectricMinerComponent)
                    .GetField("_openableInventoryItemDataStoreService", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(originalMiner);
            var loadedRemainingMillSecond =
                (int)typeof(VanillaElectricMinerComponent)
                    .GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(loadedMiner);

            Assert.AreEqual(inventory.GetItem(0), loadedInventory.GetItem(0));
            Assert.AreEqual(inventory.GetItem(1), loadedInventory.GetItem(1));
            Assert.AreEqual(inventory.GetItem(2), loadedInventory.GetItem(2));
            Assert.AreEqual(originalRemainingMillSecond, loadedRemainingMillSecond);
        }
    }
}