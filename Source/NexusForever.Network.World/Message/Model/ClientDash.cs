using NexusForever.Network.Message;
using NexusForever.Game.Static.Entity.Movement;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientActionDash)]
    public class ClientActionDash : IReadable
    {
        public DashDirection Direction { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Direction = reader.ReadEnum<DashDirection>(3u);
        }
    }
}