using KafkaReader.Models;
using System.Collections.Generic;

namespace KafkaReader.Messages
{
    class OffsetMessageResponse : ResponseKafkaMessage, IParsable
    {
        public List<TopicPartitionResponse> TopicPartitionResponseList { get; set; }
        public short ErrorCode { get; set; }

        public OffsetMessageResponse()
        {
            TopicPartitionResponseList = new List<TopicPartitionResponse>();
        }

        public void Parse()
        {
            var responses_size = _data.GetInt();
            for(var i = 0; i < responses_size; i++)
            {
                var topicPartitionResponse = new TopicPartitionResponse
                {
                    Topic = _data.GetString()
                };
                var partition_responses_size = _data.GetInt();
                for(var j = 0; j < partition_responses_size; j++)
                {
                    topicPartitionResponse.PartitionResponse.Add(new PartitionResponse
                    {
                        Partition = _data.GetInt(),
                        Offset = _data.GetLong(),
                        MetaData = _data.GetString(),
                        ErrorCode = _data.GetShort()
                    });
                }
                TopicPartitionResponseList.Add(topicPartitionResponse);
            }
        }
    }
}
