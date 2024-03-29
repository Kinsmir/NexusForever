using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientRewardTrackChoice)]
    public class ClientRewardTrackChoice : IReadable
    {
        public ushort RewardId { get; private set; } // 14
        public uint Index { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RewardId = reader.ReadUShort(14u);
            Index = reader.ReadUInt();
        }
    }
}