using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChangePetStance)]
    public class ServerChangePetStance : IWritable
    {
        public uint PetUnitId { get; set; }
        public byte Stance { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PetUnitId);
            writer.Write(Stance, 5u);
        }
    }
}