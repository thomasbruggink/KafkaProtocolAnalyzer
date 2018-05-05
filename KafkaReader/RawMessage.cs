using PcapDotNet.Packets;

namespace KafkaReader
{
    class RawMessage
    {
        private Packet _packet;
        public Struct Struct { get; set; }
        public int Size { get; set; }

        public RawMessage(Packet packet)
        {
            _packet = packet;
            Struct = new Struct(packet);
            Size = Struct.GetInt();
        }

        public bool IsComplete()
        {
            return Size == Struct.GetSize() - 4;
        }

        public bool IsRequestMessage()
        {
            return _packet.IpV4.Tcp.DestinationPort == 9092;
        }
    }
}
