using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontRequestPurchaseHistory)]
    public class ClientStorefrontRequestPurchaseHistory : IReadable
    {
        public uint UnitId { get; private set; }

        public void Read(GamePacketReader reader)
        {
        }
    }
}