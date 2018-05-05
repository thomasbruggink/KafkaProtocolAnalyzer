namespace KafkaReader.Models
{
    class PartitionResponse
    {
        public int Partition { get; set; }
        public long Offset { get; set; }
        public string MetaData { get; set; }
        public short ErrorCode { get; set; }
    }
}
