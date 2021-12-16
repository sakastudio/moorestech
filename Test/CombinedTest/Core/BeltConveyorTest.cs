﻿using System;
using Core.Block;
using Core.Block.BeltConveyor.util;
using Core.Block.BlockInventory;
using Core.Block.Config;
using Core.Item;
using Core.Item.Config;
using Core.Item.Util;
using Core.Update;
using NUnit.Framework;
using Test.Util;

namespace Test.CombinedTest.Core
{
    public class BeltConveyorTest
    {
        private ItemStackFactory _itemStackFactory;
        [SetUp]
        public void Setup()
        {
            _itemStackFactory = new ItemStackFactory(new TestItemConfig());
        }
        
        
        //一定個数以上アイテムが入らないテストした後、正しく次に出力されるかのテスト
        [Test]
        public void FullInsertAndChangeConnectorBeltConveyorTest()
        {
            var random = new Random(4123);
            for (int i = 0; i < 5; i++)
            {
                int id = random.Next(0, 10);
                var conf = BeltConveyorConfig.GetBeltConveyorData(0);
                var item = _itemStackFactory.Create(id, conf.BeltConveyorItemNum + 1);
                var beltConveyor = BeltConveyorFactory.Create(1, Int32.MaxValue,new NullIBlockInventory(),_itemStackFactory);

                var endTime = DateTime.Now.AddMilliseconds(conf.TimeOfItemEnterToExit);
                while ( DateTime.Now < endTime.AddSeconds(0.2))
                { 
                    item = beltConveyor.InsertItem(item);
                    GameUpdate.Update();
                }
                Assert.AreEqual(item.Amount,1);

                var dummy = new DummyBlockInventory();
                beltConveyor.ChangeConnector(dummy);
                GameUpdate.Update();
                
                Assert.AreEqual(_itemStackFactory.Create(id,1).ToString(),dummy.InsertedItems[0].ToString());
            }
        }
        
        //一個のアイテムが入って正しく搬出されるかのテスト
        [Test]
        public void InsertBeltConveyorTest()
        {
            var random = new Random(4123);
            for (int i = 0; i < 5; i++)
            {
                int id = random.Next(1, 11);
                int amount = random.Next(1, 10);
                var item = _itemStackFactory.Create(id, amount);
                var dummy = new DummyBlockInventory(1);
                var beltConveyor = BeltConveyorFactory.Create(0, Int32.MaxValue,dummy,_itemStackFactory);

                var expectedEndTime = DateTime.Now.AddMilliseconds(
                    BeltConveyorConfig.GetBeltConveyorData(0).TimeOfItemEnterToExit);
                var outputItem = beltConveyor.InsertItem(item);
                while (!dummy.IsItemExists)
                {
                    GameUpdate.Update();
                }
                Assert.True(DateTime.Now <= expectedEndTime.AddSeconds(0.2));
                Assert.True(expectedEndTime.AddSeconds(-0.2) <= DateTime.Now);
                
                Assert.True(outputItem.Equals(_itemStackFactory.Create(id,amount-1)));
                var tmp = _itemStackFactory.Create(id, 1);
                Console.WriteLine($"{tmp} {dummy.InsertedItems[0]}");
                Assert.AreEqual(tmp.ToString(),dummy.InsertedItems[0].ToString());
            }
        }
        //ベルトコンベアのインベントリをフルにするテスト
        [Test]
        public void FullInsertBeltConveyorTest()
        {
            var random = new Random(4123);
            for (int i = 0; i < 5; i++)
            {
                int id = random.Next(1, 11);
                var conf = BeltConveyorConfig.GetBeltConveyorData(0);
                var item = _itemStackFactory.Create(id, conf.BeltConveyorItemNum + 1);
                var dummy = new DummyBlockInventory(conf.BeltConveyorItemNum);
                var beltConveyor = BeltConveyorFactory.Create(0, Int32.MaxValue,dummy,_itemStackFactory);

                while (!dummy.IsItemExists)
                { 
                    item = beltConveyor.InsertItem(item);
                    GameUpdate.Update();
                }
                
                Assert.True(item.Equals(_itemStackFactory.Create(id,0)));
                var tmp = _itemStackFactory.Create(id, conf.BeltConveyorItemNum);
                Assert.True(dummy.InsertedItems[0].Equals(tmp));
            }
        }
        //二つのアイテムが入ったとき、一方しか入らないテスト
        [Test]
        public void Insert2ItemBeltConveyorTest()
        {
            var random = new Random(4123);
            for (int i = 0; i < 5; i++)
            {
                //必要な変数を作成
                var item1 = _itemStackFactory.Create(random.Next(1,11), random.Next(1,10));
                var item2 = _itemStackFactory.Create(random.Next(1,11), random.Next(1,10));

                var beltConveyor = BeltConveyorFactory.Create(0, Int32.MaxValue,new DummyBlockInventory(),_itemStackFactory);

                var item1Out = beltConveyor.InsertItem(item1);
                var item2Out = beltConveyor.InsertItem(item2);

                Assert.True(item1Out.Equals(item1.SubItem(1)));
                Assert.True(item2Out.Equals(item2));
            }
        }
    }
}