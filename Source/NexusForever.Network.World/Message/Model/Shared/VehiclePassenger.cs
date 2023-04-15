﻿using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class VehiclePassenger : IWritable
    {
        public VehicleSeatType SeatType { get; set; }
        public byte SeatPosition { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SeatType, 2u);
            writer.Write(SeatPosition, 3u);
            writer.Write(UnitId);
        }
    }
}