using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientAccountItemBind)]
    public class ClientAccountItemBind : IReadable
    {
        public ulong UserInventoryId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UserInventoryId  = reader.ReadULong();
        }
    }
}