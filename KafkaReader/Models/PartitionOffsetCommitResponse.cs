namespace KafkaReader.Models
{
    class PartitionOffsetCommitResponse
    {
        public int Partition { get; set; }
        public short ErrorCode { get; set; }
    }
}
