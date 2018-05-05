namespace KafkaReader
{
    class RequestHeader
    {
        public short ApiKey { get; set; }
        public short ApiVersion { get; set; }
        public int CorrelationId { get; set; }
        public string ClientId { get; set; }

        public RequestHeader(Struct data)
        {
            ApiKey = data.GetShort();
            ApiVersion = data.GetShort();
            CorrelationId = data.GetInt();
            ClientId = data.GetString();
        }
    }
}
