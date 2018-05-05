using System.Collections.Generic;

namespace KafkaReader.Models
{
    class TopicPartitions
    {
        public string Topic { get; set; }
        public List<int> Partitions { get; set; }

        public TopicPartitions()
        {
            Partitions = new List<int>();
        }
    }
}
