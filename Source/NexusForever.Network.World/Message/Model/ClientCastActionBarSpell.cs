using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCastActionBarSpell)]
    public class ClientCastActionBarSpell : IReadable
    {
        public uint ClientUniqueId { get; private set; }
        public byte ActionBarSetIndex { get; private set; } // 4
        public ShortcutSet WhichSet { get; private set; } // 4
        public uint TargetUnitId { get; private set; }
        public Position TargetPosition { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId      = reader.ReadUInt();
            ActionBarSetIndex   = reader.ReadByte(4u);
            WhichSet            = reader.ReadEnum<ShortcutSet>(4u);
            TargetUnitId        = reader.ReadUInt();

            TargetPosition.Read(reader);
        }
    }
}