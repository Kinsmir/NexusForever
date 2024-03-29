using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingEnterInside)]
    public class ClientHousingEnterInside : IReadable
    {
        public ushort RealmId { get; private set; } // 14u
        public ulong ResidenceId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId  = reader.ReadUShort(14u);
            ResidenceId = reader.ReadULong();
        }
    }
}