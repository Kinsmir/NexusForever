using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPvpModeSet)]
    public class ClientPvpModeSet : IReadable
    {
        public bool PvpMode { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PvpMode = reader.ReadBit();
        }
    }
}