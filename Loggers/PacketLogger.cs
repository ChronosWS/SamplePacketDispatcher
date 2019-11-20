using System;
using SamplePacketDispatcher.Contracts;
using Serilog;

namespace SamplePacketDispatcher.Loggers
{
    /// <summary>
    /// This represents an implementation of a specialized packet logger, which may record
    /// packets for diagnostic purposes.  This simplistic logger performs no deep serialization
    /// of the packet. A more useful version might ask packet handlers (or the packets themselves)
    /// to produce an analysis-friendly output, such as parsing a compact data format into
    /// a human readable form and logging that. The correlation ID can be matched up with the
    /// primary log stream to provide additional context for analysis.
    /// </summary>
    /// <remarks>
    /// This sample implementation logs to the console for simplicity. It could in fact log to
    /// a completely separate system. This method might also be async with a fire-and-forget
    /// pattern, as the primary packet processing system probably should not wait for these
    /// log events to complete.
    /// </remarks>
    public class PacketLogger : IPacketLogger
    {
        private readonly ILogger _logger;
        public PacketLogger(ILogger logger)
        {
            _logger = logger;
        }

        void IPacketLogger.LogPacket(PacketLogReason reason, Guid correlationId, IPacket packet)
        {
            _logger.Information(
                "PACKET LOG: CID:{correlationId} {type} {id} {@data}",
                packet.GetType().Name,
                correlationId.ToString(),
                packet.Id,
                packet.Data);
        }
    }
}