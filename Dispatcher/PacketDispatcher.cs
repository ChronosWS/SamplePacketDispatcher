using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SamplePacketDispatcher.Contracts;
using Serilog;

namespace SamplePacketDispatcher.Dispatcher
{
    /// <summary>
    /// The packet dispatcher
    /// </summary>
    /// <remarks>
    /// The dispatcher acquires its packet handlers from dependency injection, and maps packets
    /// to their handlers by way of a dictionary (from id => handler). Registering a handler
    /// for the same ID twice results in an intentional run-time exception as the system is
    /// considered to be mis-configured.
    /// </remarks>
    public class PacketDispatcher : IPacketDispatcher
    {
        private readonly ILogger _logger;
        private readonly IPacketLogger _packetLogger;
        private readonly Dictionary<int, IPacketHandler> _handlers = new Dictionary<int, IPacketHandler>();
        public PacketDispatcher(
            ILogger logger,
            IPacketLogger packetLogger,
            IEnumerable<IPacketHandler> handlers)
        {
            _logger = logger;
            _packetLogger = packetLogger;

            foreach (var handler in handlers)
            {
                if (_handlers.TryGetValue(handler.HandledPacketId, out var existingHandler))
                {
                    _logger.Error(
                        "Cannot register packet handler of type {handlerType} with packet id {id} because that id is already registered to handler type {existingHandler}",
                        handler.GetType().Name,
                        handler.HandledPacketId,
                        existingHandler.GetType().Name);
                    throw new InvalidOperationException($"Cannot register packet handler of type {handler.GetType().Name} with packet id {handler.HandledPacketId} because that id is already registered to handler type {existingHandler.GetType().Name}");
                }

                _handlers.Add(handler.HandledPacketId, handler);
            }
        }

        /// <summary>
        /// Dispatches a packet
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// This dispatcher is fault tolerant to:
        /// 1. Null packets
        /// 2. Packets with no associated handlers
        /// 3. Packets which cannot be handled by their registered handlers
        /// 
        /// This dispatcher is thread-safe as long as the loggers and handlers are thread-safe.
        /// We use correlation ids when we emit errors to match our packet log with our
        /// general log stream.
        /// 
        /// This sample includes a "bug" on packets of a ID 5 to illustrate how exceptions
        /// in the dispatcher itself are handled by the caller.
        /// </remarks>
        public async Task DispatchAsync(IPacket packet, CancellationToken cancellationToken)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Id == 5)
            {
                throw new Exception("Oh noes, a bug in the dispatcher!");
            }

            if (_handlers.TryGetValue(packet.Id, out var handler))
            {
                try
                {
                    await handler.ProcessPacketAsync(packet, cancellationToken);
                }
                catch (Exception ex)
                {
                    Guid correlationId = Guid.NewGuid();
                    _logger.Error(ex,
                        "CID:{correlationId} Failed handling packet with id {packetId} and type {packetType}",
                        correlationId.ToString(),
                        packet.Id,
                        packet.GetType().Name);
                    _packetLogger.LogPacket(PacketLogReason.Error, correlationId, packet);
                }
            }
            else
            {
                // Non-existent handler case
                Guid correlationId = Guid.NewGuid();
                _logger.Error(
                    "CID:{correlationId} Unexpected packet with id {packetId} and type {packetType}",
                    correlationId.ToString(),
                    packet.Id,
                    packet.GetType().Name);
                _packetLogger.LogPacket(PacketLogReason.Error, correlationId, packet);
            }
        }
    }
}