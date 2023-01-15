using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Block.BlockFactory;
using Core.Block.Blocks.Miner;
using Core.Block.Config;
using Core.Inventory;
using Core.Item;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server;
using Server.Boot;


using Test.Module.TestMod;

namespace Test.UnitTest.Core.Block
{
    public class MinerSaveLoadTest
    {
        private const int MinerId = UnitTestMod.MinerId;
        
        [Test]
        public void SaveLoadTest()
        {
            var (_, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create(TestModDirectory.ForUnitTestModDirectory);
            var blockFactory = serviceProvider.GetService<BlockFactory>();
            ulong minerHash = serviceProvider.GetService<IBlockConfig>().GetBlockConfig(MinerId).BlockHash;
            
            var originalMiner = blockFactory.Create(MinerId, 1);
            int originalRemainingMillSecond = 350;
            
            var inventory = 
                (OpenableInventoryItemDataStoreService)typeof(VanillaMiner).
                    GetField("_openableInventoryItemDataStoreService", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(originalMiner);
            inventory.SetItem(0,1,1);
            inventory.SetItem(2,4,1);
            typeof(VanillaMiner).
                GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic).
                SetValue(originalMiner,originalRemainingMillSecond);

            
            
            var json = originalMiner.GetSaveState();
            Console.WriteLine(json);
            
            
            var loadedMiner = blockFactory.Load(minerHash, 1,json);
            var loadedInventory = 
                (OpenableInventoryItemDataStoreService)typeof(VanillaMiner).
                    GetField("_openableInventoryItemDataStoreService", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(originalMiner);
            var loadedRemainingMillSecond = 
                (int)typeof(VanillaMiner).
                    GetField("_remainingMillSecond", BindingFlags.Instance | BindingFlags.NonPublic).
                    GetValue(loadedMiner);
            
            Assert.AreEqual(inventory.GetItem(0),loadedInventory.GetItem(0));
            Assert.AreEqual(inventory.GetItem(1),loadedInventory.GetItem(1));
            Assert.AreEqual(inventory.GetItem(2),loadedInventory.GetItem(2));
            Assert.AreEqual(originalRemainingMillSecond,loadedRemainingMillSecond);
        }
    }
}