using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamplePacketDispatcher.Contracts
{
    /// <summary>
    /// Represents a component which can process packets of a specific packet ID
    /// </summary>
    /// <remarks>
    /// This design is suitable for the case where a given packet handler implementation
    /// is intended to handle exactly one packet type. If the design considerations
    /// suggest that some handlers would be well suited to handling multiple packet
    /// types, adapters could be registered in dependency injection that map the common
    /// type to any number of IPacketHandler implementations.
    /// </remarks>
    public interface IPacketHandler
    {
        int HandledPacketId { get; }
        Task ProcessPacketAsync(IPacket packet, CancellationToken cancellationToken);
    }
}