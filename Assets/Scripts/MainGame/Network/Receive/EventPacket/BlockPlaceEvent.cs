﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Network.Event;
using MainGame.Network.Util;
using UnityEngine;

namespace MainGame.Network.Receive.EventPacket
{
    public class BlockPlaceEvent : IAnalysisEventPacket
    {
        readonly NetworkReceivedChunkDataEvent _networkReceivedChunkDataEvent;

        public BlockPlaceEvent(INetworkReceivedChunkDataEvent networkReceivedChunkDataEvent)
        {
            _networkReceivedChunkDataEvent = networkReceivedChunkDataEvent as NetworkReceivedChunkDataEvent;
        }

        public void Analysis(List<byte> packet)
        {
            var bytes = new ByteArrayEnumerator(packet);
            bytes.MoveNextToGetShort();
            bytes.MoveNextToGetShort();
            var x = bytes.MoveNextToGetInt();
            var y = bytes.MoveNextToGetInt();
            var blockId = bytes.MoveNextToGetInt();

            var direction = bytes.MoveNextToGetByte() switch
            {
                0 => BlockDirection.North,
                1 => BlockDirection.East,
                2 => BlockDirection.South,
                3 => BlockDirection.West,
                _ => BlockDirection.North
            };

            //ブロックをセットする
            _networkReceivedChunkDataEvent.InvokeBlockUpdateEvent(new OnBlockUpdateEventProperties(new Vector2Int(x,y), blockId,direction));
        }
    }
}