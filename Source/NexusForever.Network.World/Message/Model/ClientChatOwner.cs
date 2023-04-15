﻿using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientChatOwner)]
    public class ClientChatOwner : IReadable
    {
        public Channel Channel { get; private set; }
        public string CharacterName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Channel = new Channel();
            Channel.Read(reader);
            CharacterName = reader.ReadWideString();
        }
    }
}