namespace SamplePacketDispatcher.Contracts
{
    /// <summary>
    /// Represents a generic packet with an ID and binary data
    /// </summary>
    /// <remarks>
    /// Note that this packet implementation is not "efficient" in that it requires
    /// a copy of packet data (via the byte[]). More efficient binary packet processing
    /// can occur using network streams operating on Span<>s, which require no copying.
    /// </remarks>
    public interface IPacket
    {
        int Id { get; set; }
        byte[] Data { get; set; }
    }
}