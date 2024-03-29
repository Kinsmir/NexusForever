using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRemodelInterior)]
    public class ClientHousingRemodelInterior : IReadable
    {
        public uint[] Unknown0 { get; private set; } = new uint[6];
        public List<DecorInfo> Remodels { get; private set; } = new List<DecorInfo>();

        public void Read(GamePacketReader reader)
        {
            for (int i = 0; i < Unknown0.Length; i++)
                Unknown0[i] = reader.ReadUInt();

            for (int i = 0; i < Unknown0.Length; i++)
            {
                var decor = new DecorInfo();
                decor.Read(reader);
                Remodels.Add(decor);
            }
        }
    }
}