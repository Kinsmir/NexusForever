using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemRuneSocketReroll)]
    public class ClientItemRuneSocketReroll : IReadable
    {
        public ulong Guid { get; private set; }
        public uint SocketIndex { get; private set; }
        //public RuneType RuneType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadULong();
            SocketIndex = reader.ReadUInt();
            //RuneType = reader.ReadEnum<RuneType>(5u);
        }
    }
}