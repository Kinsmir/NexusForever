using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterBindpoint)]
    public class ServerCharacterBindpoint : IWritable
    {
        public ushort BindpointId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BindpointId);
        }
    }
}