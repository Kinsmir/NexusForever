﻿using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerLootNotify)]
    public class ServerLootNotify : IWritable
    {
        public uint UnitId { get; set; }
        public uint Unknown0 { get; set; }
        public bool Explosion { get; set; }
        //public List<LootItem> LootItems { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Unknown0);
            writer.Write(Explosion);
            //writer.Write(LootItems.Count);
            //LootItems.ForEach(i => i.Write(writer));
        }
    }
}