﻿using System.Linq;
using MainGame.Network.Settings;
using MessagePack;
using Server.Protocol.PacketResponse;

namespace MainGame.Network.Send
{
    public class SendGetMapObjectProtocolProtocol
    {
        
        private readonly ISocketSender _socketSender;
        private readonly int _playerId;

        
        public SendGetMapObjectProtocolProtocol(PlayerConnectionSetting playerConnectionSetting,ISocketSender socketSender)
        {
            _socketSender = socketSender;
            _playerId = playerConnectionSetting.PlayerId;
        }

        public void Send(int instanceId)
        {
            _socketSender.Send(MessagePackSerializer.Serialize(new GetMapObjectProtocolProtocolMessagePack(_playerId,instanceId)).ToList());
        }
    }
}