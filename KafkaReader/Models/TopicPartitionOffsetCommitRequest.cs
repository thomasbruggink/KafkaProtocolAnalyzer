using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaReader.Models
{
    class TopicPartitionOffsetCommitRequest
    {
        public string Topic { get; set; }
        public List<PartitionOffsetCommitRequest> PartitionOffsetCommitList { get; set; }

        public TopicPartitionOffsetCommitRequest()
        {
            PartitionOffsetCommitList = new List<PartitionOffsetCommitRequest>();
        }
    }
}
