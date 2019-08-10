using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka_POC.Service
{
    public class KafkaConsumer
    {
        private const int commitPeriod = 5;

        private IEnumerable<string> topics = new string[] { "Basic" };
        private readonly ConsumerConfig _consumerConfing;

        public KafkaConsumer(ConsumerConfig consumerConfing)
        {
            _consumerConfing = consumerConfing;
        }

        public Task ConsumeAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Consume(cancellationToken));
        }

        private void Consume(CancellationToken cancellationToken)
        {
            // This example is based in Confluent kafka: https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/Consumer/Program.cs
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfing)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    //
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(topics);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);

                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine(
                                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                // The Commit method sends a "commit offsets" request to the Kafka
                                // cluster and synchronously waits for the response. This is very
                                // slow compared to the rate at which the consumer is capable of
                                // consuming messages. A high performance application will typically
                                // commit offsets relatively infrequently and be designed handle
                                // duplicate messages in the event of failure.
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }
    }
}