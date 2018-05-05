using PcapDotNet.Packets;
using System;
using System.Text;

namespace KafkaReader
{
    class Struct
    {
        private byte[] _payload;
        private int _offset;

        public Struct(byte[] payload)
        {
            _payload = payload;
            _offset = 0;
        }

        public Struct(Packet packet)
        {
            using (var stream = packet.IpV4.Tcp.Payload.ToMemoryStream())
            {
                _payload = new byte[packet.IpV4.Tcp.PayloadLength];
                stream.Read(_payload, 0, _payload.Length);
            }
            _offset = 0;
        }

        public int GetInt()
        {
            return BitConverter.ToInt32(Get(4), 0);
        }

        public long GetLong()
        {
            return BitConverter.ToInt64(Get(8), 0);
        }

        public short GetShort()
        {
            return BitConverter.ToInt16(Get(2), 0);
        }

        public byte GetByte()
        {
            return Get(1)[0];
        }

        public string GetString()
        {
            var size = GetShort();
            if (size == 0 || size == -1)
                return string.Empty;
            var result = Get(size);
            Array.Reverse(result);
            return Encoding.ASCII.GetString(result);
        }

        public void Extend(Struct data)
        {
            var newPayload = new byte[_payload.Length + data._payload.Length];
            Buffer.BlockCopy(_payload, 0, newPayload, 0, _payload.Length);
            Buffer.BlockCopy(data._payload, 0, newPayload, _payload.Length, data._payload.Length);
            _payload = newPayload;
        }

        public int GetSize()
        {
            return _payload.Length;
        }

        private byte[] Get(int length)
        {
            if (length + _offset > _payload.Length)
                throw new ArgumentOutOfRangeException("Not enough data left to read");
            var response = new byte[length];
            Buffer.BlockCopy(_payload, _offset, response, 0, length);
            _offset += length;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(response);
            return response;
        }
    }
}
