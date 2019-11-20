using System.Threading;
using System.Threading.Tasks;

namespace SamplePacketDispatcher.Contracts
{
    /// <summary>
    /// Represents a packet dispatcher
    /// </summary>
    public interface IPacketDispatcher
    {
        Task DispatchAsync(IPacket packet, CancellationToken cancellationToken);
    }
}