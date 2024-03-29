using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInviteResponse)]
    public class ClientGroupInviteResponse : IReadable
    {
        public ulong GroupId { get; set; }
        public uint Response { get; set; }
        public bool Unknown0 { get; set; }

        public void Read(GamePacketReader reader)
        {
            GroupId = reader.ReadULong();
            Response = reader.ReadUInt();
            Unknown0 = reader.ReadBit();
        }
    }
}