using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaReader.Models
{
    class PartitionOffsetCommitRequest
    {
        public int Partition { get; set; }
        public long Offset { get; set; }
        public string MetaData { get; set; }
    }
}
