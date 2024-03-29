using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInvite)]
    public class ClientGroupInvite : IReadable
    {
        public string PlayerName { get; private set; }
        public string Unknown0 { get; set; }

        public void Read(GamePacketReader reader)
        {
            PlayerName = reader.ReadWideString();
            Unknown0 = reader.ReadWideString();
        }
    }
}