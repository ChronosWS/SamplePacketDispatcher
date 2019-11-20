# Specifications


1. What is the specification for the packet format?  Binary `(ID, LEN, DATA)`, JSON, something else?
   * `int id, byte[] data`
2. What sort of pipeline should I assume I am receiving them from, or does it matter for this question (socket/byte stream, async enumeration of fully-formed packets, calls to `DispatchPacket(Packet packet)`, etc.) and do I need to be responsible for handling exceptional stream conditions or is this strictly packet dispatch?  If `DispatchPacket()`, do I need to guarantee thread safety?
   * We will assume we are getting called with on `DispatchPacket()`.
3. Can a single packet be routed to multiple recipients, and if so, is there an ordering which I need to abide by, and can handlers cancel further dispatches for a packet, etc.?
   * No, packets may only be handled by a single registered packet handler. It is a setup exception for two packet handlers to be registered for the same packet id
4. Are handlers assumed to be registered at the beginning of the app and remain unchanged for the lifetime of the app, or can they be registered/unregistered during the operation of the dispatcher (or even by packet handlers themselves in response to receiving a packet)?
   * Packet handlers are registered once and remain unchanged.
5. Are handlers registered by a separate mechanism which associates packet IDs with the handler that should handle them or do packet handlers contain their own registration information (or do you care)?
   * Packet handlers will be registered in a "registration" method, to ease maintenance and avoid magic.
6. Do I need to deal with outgoing packets in any way, or do packet handlers produce any output which needs to be returned to the caller (e.g. in the case of DispatchPacket())?
   * Outgoing packets are out-of-scope
7. Are packets assumed to be received "fully formed", or can they be split among several smaller packets, necessitating reassembly upon reception?  If the latter, how do I distinguish these partial packets from full ones? What rule do I use to expire incomplete packets? Can packet parts be received out-of-order?
   * Packets are "fully-formed"
8. What are the desired "malformed packet", "incomplete packet" and "exception in the packet handler" outcomes?
   * Errors should be logged for offline analysis and discarded
9. Are packets expected to be handled serially, or can packet handler calls overlap?  If the latter, can I assume a packet handler instance is either thread-safe or stateless, such that it is safe to call it in parallel with itself, or do I need to serialize calls to certain packet handlers to avoid thread-safety limitations?
   * Packets are handled serially
