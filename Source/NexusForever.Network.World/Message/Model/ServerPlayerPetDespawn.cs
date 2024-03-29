using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerPetDespawn)]
    public class ServerPlayerPetDespawn : IWritable
    {
        public uint Guid { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
        }
    }
}