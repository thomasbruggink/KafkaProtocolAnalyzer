using KafkaReader.Models;
using System.Collections.Generic;

namespace KafkaReader.Messages
{
    class FetchRequest : RequestKafkaMessage, IParsable
    {
        public int ReplicaId { get; set; }
        public int MaxWaitTime { get; set; }
        public int MinBytes { get; set; }
        public int MaxBytes { get; set; }
        public byte IsolationLevel { get; set; }
        public List<FetchTopicRequest> Topics { get; set; }

        public FetchRequest()
        {
            Topics = new List<FetchTopicRequest>();
        }


        //Fetch Request(Version: 5) => replica_id max_wait_time min_bytes max_bytes isolation_level[topics]
        //  replica_id => INT32
        //  max_wait_time => INT32
        //  min_bytes => INT32
        //  max_bytes => INT32
        //  isolation_level => INT8
        //  topics => topic[partitions]
        //    topic => STRING
        //    partitions => partition fetch_offset log_start_offset max_bytes
        //      partition => INT32
        //      fetch_offset => INT64
        //      log_start_offset => INT64
        //      max_bytes => INT32
        public void Parse()
        {
            ReplicaId = _data.GetInt();
            MaxWaitTime = _data.GetInt();
            MinBytes = _data.GetInt();
            MaxBytes = _data.GetInt();
            IsolationLevel = _data.GetByte();

            var topics_size = _data.GetInt();
            for (var i = 0; i < topics_size; i++)
            {
                var topicRequest = new FetchTopicRequest
                {
                    Topic = _data.GetString()
                };
                var partition_size = _data.GetInt();
                for (var j = 0; j < partition_size; j++)
                {
                    var partition = _data.GetInt();
                    var offset = _data.GetLong();
                    var maxBytes = _data.GetInt();
                    topicRequest.Partitions.Add(new FetchPartitionRequest
                    {
                        Partition = partition,
                        FetchOffset = offset,
                        MaxBytes = maxBytes
                    });
                }
                Topics.Add(topicRequest);
            }
        }
    }
}
