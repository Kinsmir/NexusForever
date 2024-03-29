using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGachaRollRequest)]
    public class ClientGachaRollRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
        }
    }
}