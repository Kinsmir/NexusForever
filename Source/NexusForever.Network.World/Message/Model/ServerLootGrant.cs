using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerLootGrant)]
    public class ServerLootGrant : IWritable
    {
        public uint UnitId { get; set; }
        public uint LooterId { get; set; }
        //public LootItem LootItem { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(LooterId);
            //LootItem.Write(writer);
        }
    }
}