using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerPet)]
    public class ServerPlayerPet : IWritable
    {
        public ServerPlayerCreate.Pet Pet { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Pet.Write(writer);
        }
    }
}
