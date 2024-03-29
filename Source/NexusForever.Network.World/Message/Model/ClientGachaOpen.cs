using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGachaOpen)]
    public class ClientGachaOpen : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}