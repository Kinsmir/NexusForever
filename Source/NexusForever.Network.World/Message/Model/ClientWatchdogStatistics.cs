using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientWatchdogStatistics)]
    public class ClientWatchdogStatistics : IReadable
    {
        public ulong RandomValue { get; private set; }
        public ulong Seed { get; private set; }
        public int LongestTimeBetweenWatchdogloops { get; private set; }
        public float TimeToMiddleofcircularBuffer { get; private set; }
        public float TimeBetweenWatchdogRunsWeightedAverage { get; private set; }
        public float WeightedError { get; private set; }
        public uint UpdatePlayerRelated { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Seed = reader.ReadULong();
            RandomValue = reader.ReadULong();
            LongestTimeBetweenWatchdogloops = reader.ReadInt();
            TimeToMiddleofcircularBuffer = reader.ReadSingle();
            TimeBetweenWatchdogRunsWeightedAverage = reader.ReadSingle();
            WeightedError = reader.ReadSingle();
            UpdatePlayerRelated = reader.ReadUInt();
        }
        
    }
}