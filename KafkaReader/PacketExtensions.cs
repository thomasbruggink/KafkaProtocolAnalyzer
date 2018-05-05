using PcapDotNet.Packets;

namespace KafkaReader
{
    static class PacketExtensions
    {
        public static string GetHash(this Packet packet)
        {
            return packet.IpV4.Tcp.SourcePort.ToString() + "|" + packet.IpV4.Tcp.AcknowledgmentNumber;
        }

        public static bool MatchesStream(this Packet left, Packet right)
        {
            return left.IpV4.Source == right.IpV4.Destination &&
                   left.IpV4.Destination == right.IpV4.Source &&
                   left.IpV4.Tcp.SourcePort == right.IpV4.Tcp.DestinationPort &&
                   left.IpV4.Tcp.DestinationPort == right.IpV4.Tcp.SourcePort &&
                   left.IpV4.Tcp.AcknowledgmentNumber != right.IpV4.Tcp.AcknowledgmentNumber;
        }
    }
}
