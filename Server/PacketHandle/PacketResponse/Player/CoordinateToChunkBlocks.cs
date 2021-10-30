﻿using Server.Const;
using World.DataStore;
using World.Util;

namespace Server.PacketHandle.PacketResponse.Player
{
    public static class CoordinateToChunkBlocks
    {
        public static int[,] Convert(Coordinate coordinate,WorldBlockDatastore worldBlockDatastore)
        {
            //その座標のチャンクの原点
            var x = coordinate.x / ChunkResponseConst.ChunkSize * ChunkResponseConst.ChunkSize;
            var y = coordinate.y / ChunkResponseConst.ChunkSize * ChunkResponseConst.ChunkSize;
            
            var blocks = new int[ChunkResponseConst.ChunkSize,ChunkResponseConst.ChunkSize];

            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    blocks[i, j] = worldBlockDatastore.GetBlock(
                        x + i,
                        y+ j).GetBlockId();
                }                
            }

            return blocks;
        }

        public static int[,] Convert(int x, int y,WorldBlockDatastore worldBlockDatastore)
        {
            return Convert(CoordinateCreator.New(x, y),worldBlockDatastore);
        }
    }
}