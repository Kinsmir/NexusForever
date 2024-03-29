using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStorefrontPurchaseGift)]
    public class ClientStorefrontPurchaseGift : IReadable
    {
        //public StorefrontPurchase StorefrontPurchase { get; private set; } = new();
        public uint Unknown1 { get; set; }
        public TargetPlayerIdentity Target { get; set; } = new TargetPlayerIdentity();
        public string Unknown2 { get; set; }

        public void Read(GamePacketReader reader)
        {
            //StorefrontPurchase.Read(reader);
            Unknown1 = reader.ReadUInt();
            Target.Read(reader);
            Unknown2 = reader.ReadWideString();
        }
    }
}