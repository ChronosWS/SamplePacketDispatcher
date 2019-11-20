using System;

namespace SamplePacketDispatcher.Contracts
{
    /// <summary>
    /// The Packet Logging reason
    /// </summary>
    public enum PacketLogReason
    {
        Debug,
        Error
    }

    /// <summary>
    /// Represents a component which knows how to log packets
    /// </summary>
    public interface IPacketLogger
    {
        void LogPacket(PacketLogReason reason, Guid correlationId, IPacket packet);
    }
}