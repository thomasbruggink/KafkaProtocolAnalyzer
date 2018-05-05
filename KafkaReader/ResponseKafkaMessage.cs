using PcapDotNet.Packets;

namespace KafkaReader
{
    class ResponseKafkaMessage
    {
        public Packet Packet { get; set; }
        public ResponseHeader Header { get; set; }
        protected Struct _data;
        protected object _internalMessage;

        protected ResponseKafkaMessage()
        {

        }

        public ResponseKafkaMessage(Packet packet, Struct data)
        {
            _data = data;
            _internalMessage = null;
            Packet = packet;
            Header = new ResponseHeader(data);
        }

        public T ReadAs<T>() where T : ResponseKafkaMessage, IParsable, new()
        {
            if (_internalMessage != null)
                return (T)_internalMessage;
            var result = new T();
            var tempCast = (ResponseKafkaMessage)result;
            tempCast._data = _data;
            tempCast.Packet = Packet;
            tempCast.Header = Header;
            result.Parse();
            _internalMessage = result;
            tempCast._internalMessage = result;
            return result;
        }
    }
}
