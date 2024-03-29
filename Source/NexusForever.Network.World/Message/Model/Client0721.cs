using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Client0721)]
    public class Client0721 : IReadable
    {

        public void Read(GamePacketReader reader)
        {
        }
    }
}