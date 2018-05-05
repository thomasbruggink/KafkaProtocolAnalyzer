using System.Collections.Generic;

namespace KafkaReader.Models
{
    class TopicPartitionOffsetCommitResponse
    {
        public string Topic { get; set; }
        public List<PartitionOffsetCommitResponse> PartitionOffsetCommitResponseList { get; set; }

        public TopicPartitionOffsetCommitResponse()
        {
            PartitionOffsetCommitResponseList = new List<PartitionOffsetCommitResponse>();
        }
    }
}
