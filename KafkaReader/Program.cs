using KafkaReader.Messages;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KafkaReader
{
    class Program
    {
        public static int TotalPacketCount = 0;
        public static List<RequestKafkaMessage> RequestMessages = new List<RequestKafkaMessage>();
        public static List<ResponseKafkaMessage> ResponseMessages = new List<ResponseKafkaMessage>();

        static void Main(string[] args)
        {
            ParseAndRead();

            //foreach (var request in RequestMessages.Where(r => r.Header.ApiKey == 1))
            //{
            //    if (!request.Header.ClientId.Equals("TelcoServiceWorker"))
            //        continue;
            //    var fetch = request.ReadAs<FetchRequest>();
            //    foreach(var topic in fetch.Topics.Where(t => t.Partitions.Any(p => p.Partition == 16)))
            //    {
            //        Console.Write($"{request.Packet.Timestamp} {topic.Topic} ");
            //        foreach(var partition in topic.Partitions.Where(p => p.Partition == 16))
            //        {
            //            Console.Write($"{request.Packet.IpV4.Tcp.SourcePort} {partition.Partition}|{partition.FetchOffset} ");
            //        }
            //        Console.WriteLine();
            //    }
            //}
            //Console.WriteLine();
            var temp = new HashSet<string>();
            foreach(var r in RequestMessages.Where(r => r.Header.ApiKey == 1))
            {
                if (!r.Header.ClientId.Equals("TelcoServiceWorker"))
                    continue;
                if (temp.Contains(r.Packet.IpV4.Tcp.SourcePort.ToString()))
                    continue;
                temp.Add(r.Packet.IpV4.Tcp.SourcePort.ToString());
            }
            foreach(var s in temp)
                Console.WriteLine(s);
            var offsetFetchMessages = RequestMessages.Where(m => m.Header.ApiKey == KafkaMessageTypes.OffsetFetch).ToList();

            // GroupId -> Topic -> Partition[] -> Offsets[]
            var data = new Dictionary<string, Dictionary<string, Dictionary<int, List<dynamic>>>>();

            Console.WriteLine(offsetFetchMessages.Count);
            foreach(var message in offsetFetchMessages)
            {
                var offsetMessage = message.ReadAs<OffsetMessageRequest>();
                var offsetResponse = offsetMessage.GetMatchingResponse<OffsetMessageResponse>();
                if(!data.ContainsKey(offsetMessage.GroupId))
                    data.Add(offsetMessage.GroupId, new Dictionary<string, Dictionary<int, List<dynamic>>>());
                foreach(var topicPartitions in offsetMessage.TopicPartitionsList)
                {
                    if (!data[offsetMessage.GroupId].ContainsKey(topicPartitions.Topic))
                        data[offsetMessage.GroupId].Add(topicPartitions.Topic, new Dictionary<int, List<dynamic>>());
                    var matchingTopicResponse = offsetResponse.TopicPartitionResponseList.First(tp => tp.Topic.Equals(topicPartitions.Topic));
                    foreach (var partition in topicPartitions.Partitions)
                    {
                        var matchingPartitionResponse = matchingTopicResponse.PartitionResponse.First(p => p.Partition == partition);
                        if (data[offsetMessage.GroupId][topicPartitions.Topic].ContainsKey(partition))
                            data[offsetMessage.GroupId][topicPartitions.Topic][partition].Add(new
                            {
                                Init = true,
                                Offset = matchingPartitionResponse.Offset,
                                TimeStamp = message.Packet.Timestamp
                            });
                        else
                            data[offsetMessage.GroupId][topicPartitions.Topic].Add(partition, new List<dynamic>
                            {
                                new
                                {
                                    Init = true,
                                    Offset = matchingPartitionResponse.Offset,
                            TimeStamp = message.Packet.Timestamp
                                }
                            });
                    }
                }
            }

            var offsetCommitMessages = RequestMessages.Where(m => m.Header.ApiKey == KafkaMessageTypes.OffsetCommit).ToList();
            Console.WriteLine(offsetCommitMessages.Count);
            var telcoMessages = 0;
            foreach(var message in offsetCommitMessages)
            {
                var commitMessage = message.ReadAs<OffsetCommitRequest>();
                if (!data.ContainsKey(commitMessage.GroupId))
                {
                    Console.WriteLine($"Skipping groupid: {commitMessage.GroupId}");
                    continue;
                }
                telcoMessages++;
                foreach (var topicPartitions in commitMessage.TopicPartitionOffsetCommitRequestList)
                {
                    foreach (var partition in topicPartitions.PartitionOffsetCommitList)
                    {
                        data[commitMessage.GroupId][topicPartitions.Topic][partition.Partition].Add(new
                        {
                            Init = false,
                            Offset = partition.Offset,
                            TimeStamp = message.Packet.Timestamp
                        });
                    }
                }
            }
            Console.WriteLine(telcoMessages);

            var current = Console.ForegroundColor;
            foreach(var group in data)
            {
                Console.WriteLine(group.Key);
                foreach(var topic in group.Value)
                {
                    Console.WriteLine($"\t{topic.Key}");
                    foreach (var partition in topic.Value.OrderBy(p => p.Key))
                    {
                        Console.Write($"\t\t{partition.Key}");
                        Console.Write($"\t");
                        foreach (var offset in partition.Value.OrderBy(o => o.TimeStamp))
                        {
                            if (offset.Init)
                                Console.ForegroundColor = ConsoleColor.Red;
                            //Console.Write($"{offset.TimeStamp} {offset.Offset} ");
                            Console.Write($" {offset.Offset} ");
                            if (offset.Init)
                                Console.ForegroundColor = current;
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void ParseAndRead()
        {
            var device = new OfflinePacketDevice(@"C:\Users\Thomas\Desktop\dumpfile.pcap");

            // Open the capture file
            using (PacketCommunicator communicator = device.Open(65536,     // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                // Read and dispatch packets until EOF is reached
                try
                {
                    communicator.ReceivePackets(0, DispatcherHandler);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static Dictionary<string, RawMessage> IncompleteMessages = new Dictionary<string, RawMessage>();

        private static void DispatcherHandler(Packet packet)
        {
            // Filter basic TCP and ignore SYNS and ACKS
            if (!packet.IpV4.Tcp.IsValid || packet.IpV4.Tcp.PayloadLength == 0)
                return;
            TotalPacketCount++;
            // Filter kafka
            if (packet.IpV4.Tcp.DestinationPort != 9092 && packet.IpV4.Tcp.SourcePort != 9092)
                return;

            var packetHash = packet.GetHash();
            var previous = IncompleteMessages.ContainsKey(packetHash) ? IncompleteMessages[packetHash] : null;
            RawMessage currentMessage;
            if (previous != null)
            {
                previous.Struct.Extend(new Struct(packet));
                if (!previous.IsComplete())
                    return;
                IncompleteMessages.Remove(packetHash);
                currentMessage = previous;
            }
            else
            {
                currentMessage = new RawMessage(packet);
                if (!currentMessage.IsComplete())
                {
                    IncompleteMessages.Add(packetHash, currentMessage);
                    return;
                }
            }

            if (currentMessage.IsRequestMessage())
                RequestMessages.Add(new RequestKafkaMessage(packet, currentMessage.Struct));
            else
                ResponseMessages.Add(new ResponseKafkaMessage(packet, currentMessage.Struct));
        }
    }
}
