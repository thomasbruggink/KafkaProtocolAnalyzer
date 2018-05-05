using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaReader.Models
{
    class FetchPartitionRequest
    {
        //      partition => INT32
        //      fetch_offset => INT64
        //      max_bytes => INT32

        public int Partition { get; set; }
        public long FetchOffset { get; set; }
        public int MaxBytes { get; set; }
    }
}
