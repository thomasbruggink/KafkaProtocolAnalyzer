using KafkaReader.Models;
using System.Collections.Generic;

namespace KafkaReader.Messages
{
    class OffsetMessageRequest : RequestKafkaMessage, IParsable
    {
        public string GroupId { get; set; }
        public List<TopicPartitions> TopicPartitionsList { get; set; }

        public OffsetMessageRequest()
        {
            TopicPartitionsList = new List<TopicPartitions>();
        }

        public void Parse()
        {
            GroupId = _data.GetString();
            var topics_size = _data.GetInt();
            for(var i = 0; i < topics_size; i++)
            {
                var topicPartition = new TopicPartitions
                {
                    Topic = _data.GetString()
                };
                var partition_size = _data.GetInt();
                for(var j = 0; j < partition_size; j++)
                {
                    var partition = _data.GetInt();
                    topicPartition.Partitions.Add(partition);
                }
                TopicPartitionsList.Add(topicPartition);
            }
        }
    }
}
