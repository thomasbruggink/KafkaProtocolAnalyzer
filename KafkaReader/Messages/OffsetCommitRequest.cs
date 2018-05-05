using KafkaReader.Models;
using System.Collections.Generic;

namespace KafkaReader.Messages
{
    class OffsetCommitRequest : RequestKafkaMessage, IParsable
    {
        public string GroupId { get; set; }
        public int GenerationId { get; set; }
        public string MemberId { get; set; }
        public List<TopicPartitionOffsetCommitRequest> TopicPartitionOffsetCommitRequestList { get; set; }

        public void Parse()
        {
            GroupId = _data.GetString();
            GenerationId = _data.GetInt();
            MemberId = _data.GetString();

            TopicPartitionOffsetCommitRequestList = new List<TopicPartitionOffsetCommitRequest>();
            var topic_size = _data.GetInt();
            for(var i = 0; i < topic_size; i++)
            {
                var topic_request = new TopicPartitionOffsetCommitRequest
                {
                    Topic = _data.GetString()
                };
                var partition_size = _data.GetInt();
                for(var j = 0; j < topic_size; j++)
                {
                    topic_request.PartitionOffsetCommitList.Add(new PartitionOffsetCommitRequest
                    {
                        Partition = _data.GetInt(),
                        Offset = _data.GetLong(),
                        MetaData = _data.GetString()
                    });
                }
                TopicPartitionOffsetCommitRequestList.Add(topic_request);
            }
        }
    }
}
