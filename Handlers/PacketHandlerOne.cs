using System.Threading.Tasks;
using Serilog;
using SamplePacketDispatcher.Contracts;
using System.Threading;

namespace SamplePacketDispatcher.Handlers
{
    /// <summary>
    /// A sample packet handler
    /// </summary>
    public class PacketHandlerOne : IPacketHandler
    {
        const int PacketId = 1;

        private readonly ILogger _logger;

        public PacketHandlerOne(ILogger logger)
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
            return Task.CompletedTask;
        }
    }
}