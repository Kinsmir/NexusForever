﻿using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupInviteResult)]
    public class ServerGroupInviteResult : IWritable
    {
        public ulong GroupId { get; set; }
        public string PlayerName { get; set; }
        //public InviteResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.WriteStringWide(PlayerName);
            //writer.Write(Result, 5u);
        }
    }
}