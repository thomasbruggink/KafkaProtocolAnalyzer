namespace KafkaReader
{
    class ResponseHeader
    {
        public int CorrelationId { get; set; }

        public ResponseHeader(Struct data)
        {
            CorrelationId = data.GetInt();
        }
    }
}
