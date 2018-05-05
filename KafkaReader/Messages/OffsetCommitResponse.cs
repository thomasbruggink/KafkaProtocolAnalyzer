using KafkaReader.Models;
using System.Collections.Generic;

namespace KafkaReader.Messages
{
    class OffsetCommitResponse : ResponseKafkaMessage, IParsable
    {
        public List<TopicPartitionOffsetCommitResponse> TopicPartitionOffsetCommitResponseList { get; set; }

        public void Parse()
        {
            TopicPartitionOffsetCommitResponseList = new List<TopicPartitionOffsetCommitResponse>();
            var topic_size = _data.GetInt();
            for (var i = 0; i < topic_size; i++)
            {
                var topic_response = new TopicPartitionOffsetCommitResponse
                {
                    Topic = _data.GetString()
                };
                var partition_size = _data.GetInt();
                for (var j = 0; j < topic_size; j++)
                {
                    topic_response.PartitionOffsetCommitResponseList.Add(new PartitionOffsetCommitResponse
                    {
                        Partition = _data.GetInt(),
                        ErrorCode = _data.GetShort()
                    });
                }
                TopicPartitionOffsetCommitResponseList.Add(topic_response);
            }
        }
    }
}
