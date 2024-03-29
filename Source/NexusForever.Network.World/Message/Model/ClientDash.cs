using NexusForever.Network.Message;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientDash)]
    public class ClientDash : IReadable
    {
        //public DashDirection Direction { get; private set; }

        public void Read(GamePacketReader reader)
        {
            //Direction = reader.ReadEnum<DashDirection>(3u);
        }
    }
}