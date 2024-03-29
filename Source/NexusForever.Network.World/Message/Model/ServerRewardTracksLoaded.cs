using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardTracksLoaded)]
    public class ServerRewardTracksLoaded : IWritable
    {
        //public List<ServerRewardTrack> RewardTracks { get; set; } = new List<ServerRewardTrack>();

        public void Write(GamePacketWriter writer)
        {
            //writer.Write(RewardTracks.Count, 32u);
            //RewardTracks.ForEach(r => r.Write(writer));
        }
    }
}