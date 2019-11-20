using System.Threading.Tasks;
using Serilog;
using SamplePacketDispatcher.Contracts;
using SamplePacketDispatcher.Packets;
using System;
using System.Threading;

namespace SamplePacketDispatcher.Handlers
{
    /// <summary>
    /// A sample packet handler which chokes on a particular bad data sequence
    /// </summary>
    public class PacketHandlerTwo : IPacketHandler
    {
        const int PacketId = 2;

        private readonly ILogger _logger;

        public PacketHandlerTwo(ILogger logger)
        {
            _logger = logger;
        }

        public int HandledPacketId => PacketId;

        public Task ProcessPacketAsync(IPacket packet, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            _logger.Information("Packet handler {handler} got packet {packet}!", HandledPacketId, packet.Id);
            if (packet.Data == TestPacket.BAD_DATA)
            {
                throw new ArgumentException("I can has error?!");
            }

            return Task.CompletedTask;
        }
    }
}