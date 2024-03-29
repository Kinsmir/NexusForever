using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPetDismiss)]
    public class ClientPetDismiss : IReadable
    {
        public uint Unknown0 { get; private set; }
        public uint Unknown1 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadUInt();
            Unknown1 = reader.ReadUInt();
        }
    }
}