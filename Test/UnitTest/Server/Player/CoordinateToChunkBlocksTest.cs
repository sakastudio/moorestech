﻿using System;
using System.Collections.Generic;
using Core.Block;
using Core.Block.BlockFactory;
using Core.Block.Config;
using Core.Block.Machine;
using Core.Block.RecipeConfig;
using Core.Item;
using Core.Item.Config;
using Game.World.Interface.Util;
using NUnit.Framework;
using Server.PacketHandle.PacketResponse.Player;
using Server.Protocol.PacketResponse.Const;
using Test.TestConfig;
using World;
using World.Event;
using IntId = World.IntId;

namespace Test.UnitTest.Server.Player
{
    public class CoordinateToChunkBlocksTest
    {
        [Test]
        public void NothingBlockTest()
        {
            var worldData = new WorldBlockDatastore(new BlockPlaceEvent());
            var b = CoordinateToChunkBlocks.Convert(CoordinateCreator.New(0,0),worldData);

            Assert.AreEqual(b.GetLength(0),ChunkResponseConst.ChunkSize);
            Assert.AreEqual(b.GetLength(1),ChunkResponseConst.ChunkSize);

            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    Assert.AreEqual(BlockConst.BlockConst.NullBlockId,b[i,j]);
                }
            }
        }

        [Test]
        public void SameBlockResponseTest()
        {
            var worldData = new WorldBlockDatastore(new BlockPlaceEvent());
            var random = new Random(3944156);
            //ブロックの設置
            for (int i = 0; i < 10000; i++)
            {
                var b = CreateMachine(random.Next(0, 500));
                worldData.AddBlock(b, random.Next(-300, 300), random.Next(-300, 300),b);
            }
            //レスポンスのチェック
            for (int l = 0; l < 100; l++)
            {
                var c = CoordinateCreator.New(
                    random.Next(-5, 5) * ChunkResponseConst.ChunkSize,
                    random.Next(-5, 5) * ChunkResponseConst.ChunkSize);
                var b = CoordinateToChunkBlocks.Convert(c,worldData);
                
                //ブロックの確認
                for (int i = 0; i < b.GetLength(0); i++)
                {
                    for (int j = 0; j < b.GetLength(1); j++)
                    {
                        Assert.AreEqual(
                            worldData.GetBlock(c.x + i, c.y + j).GetBlockId(),
                            b[i,j]);
                    }
                }
            }
        }
        

        //todo 仮実装のためのstringをenumにする
        private BlockFactory _blockFactory;
        private NormalMachine CreateMachine(int id)
        {
            if (_blockFactory == null)
            {
                var itemStackFactory = new ItemStackFactory(new TestItemConfig());
                _blockFactory = new BlockFactory(new AllMachineBlockConfig(),new VanillaIBlockTemplates(new TestMachineRecipeConfig(itemStackFactory),itemStackFactory));
            }
            var machine = _blockFactory.Create(id, IntId.NewIntId()) as NormalMachine;
            return machine;
        }
    }
}