using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardTrackUpdate)]
    public class ServerRewardTrackUpdate : IWritable
    {
        //public ServerRewardTrack RewardTrack { get; set; }

        public void Write(GamePacketWriter writer)
        {
            //RewardTrack.Write(writer);
        }
    }
}