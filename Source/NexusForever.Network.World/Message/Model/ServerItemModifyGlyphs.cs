using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerItemModifyGlyphs)]
    public class ServerItemModifyGlyphs : IWritable
    {
        public ulong ItemGuid { get; set; }
        public uint RandomGlyphData { get; set; }
        public List<uint> Glyphs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(RandomGlyphData);
            writer.Write(Glyphs.Count, 4u);
            Glyphs.ForEach(g => writer.Write(g));
        }
    }
}