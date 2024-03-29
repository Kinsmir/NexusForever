using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematic0227)]
    public class ServerCinematic0227 : IWritable
    {
        public uint Delay { get; set; }
        public uint Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Unknown0);
        }
    }
}