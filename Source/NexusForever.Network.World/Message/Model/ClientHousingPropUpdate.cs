using System.Numerics;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingPropUpdate)]
    public class ClientHousingPropUpdate : IReadable
    {
        public ushort RealmId { get; private set; } // 14u
        public ulong ResidenceId { get; private set; }
        public ulong PropId { get; private set; }
        public uint DecorId { get; private set; }
        public byte Operation { get; private set; } // 3u
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RealmId  = reader.ReadUShort(14u);
            ResidenceId = reader.ReadULong();
            PropId    = reader.ReadULong();
            DecorId   = reader.ReadUInt();
            Operation = reader.ReadByte(3u);
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}