using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server053A)]
    public class Server053A : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public long ActivePropId { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(ResidenceId);
            writer.Write(ActivePropId);
            writer.Write(UnitId);
        }
    }
}