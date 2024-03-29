using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemRuneInsert)]
    public class ClientItemRuneInsert : IReadable
    {
        public ulong Guid { get; private set; }
        public List<uint> Glyphs { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            Guid = reader.ReadULong();

            uint glyphCount = reader.ReadUInt();
            for (int i = 0; i < glyphCount; i++)
                Glyphs.Add(reader.ReadUInt());
        }
    }
}