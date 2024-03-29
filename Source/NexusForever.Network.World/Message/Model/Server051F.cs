using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server051F)]
    public class Server051F : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public uint PlotIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(ResidenceId);
            writer.Write(PlotIndex);
        }
    }
}