using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaReader.Models
{
    class FetchTopicRequest
    {
        public string Topic { get; set; }
        public List<FetchPartitionRequest> Partitions { get; set; }

        public FetchTopicRequest()
        {
            Partitions = new List<FetchPartitionRequest>();
        }

    }
}
