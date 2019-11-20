using SamplePacketDispatcher.Contracts;

namespace SamplePacketDispatcher.Packets
{
    /// <summary>
    /// This is a test packet which can be filled with arbitrary data and IDs
    /// </summary>
    public class TestPacket : IPacket
    {
        public static readonly byte[] BAD_DATA = new byte[] { 0, 1, 2 };

        public int Id { get; set; }
        public byte[] Data { get; set; }
    }
}