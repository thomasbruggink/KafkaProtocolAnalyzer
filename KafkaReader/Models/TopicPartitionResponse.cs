using System.Collections.Generic;

namespace KafkaReader.Models
{
    class TopicPartitionResponse
    {
        public string Topic { get; set; }
        public List<PartitionResponse> PartitionResponse { get; set; }

        public TopicPartitionResponse()
        {
            PartitionResponse = new List<PartitionResponse>();
        }
    }
}
