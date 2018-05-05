using PcapDotNet.Packets;
using System.Linq;

namespace KafkaReader
{
    class KafkaMessageTypes
    {
        public static short OffsetCommit = 8;
        public static short OffsetFetch = 9;
    }

    class RequestKafkaMessage
    {
        public Packet Packet { get; set; }
        public RequestHeader Header { get; set; }
        protected Struct _data;

        protected RequestKafkaMessage()
        {

        }

        public RequestKafkaMessage(Packet packet, Struct data)
        {
            _data = data;
            Packet = packet;
            Header = new RequestHeader(data);
        }

        public T ReadAs<T>() where T : RequestKafkaMessage, IParsable, new()
        {
            var result = new T();
            var tempCast = (RequestKafkaMessage)result;
            tempCast._data = _data;
            tempCast.Packet = Packet;
            tempCast.Header = Header;
            result.Parse();
            return result;
        }

        public T GetMatchingResponse<T>() where T : ResponseKafkaMessage, IParsable, new()
        {
            return Program.ResponseMessages.First(m => m.Header.CorrelationId.Equals(Header.CorrelationId) && m.Packet.MatchesStream(Packet)).ReadAs<T>();
        }
    }
}
