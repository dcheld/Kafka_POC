using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace Kafka_POC.Service
{
    public class KafkaProducer
    {
        private readonly ProducerConfig _producerConfing;
        private string topics = "Basic";

        public KafkaProducer(ProducerConfig producerConfing)
        {
            _producerConfing = producerConfing;
        }

        public async Task ProduceAsync(string value)
        {
            // This example is based in Confluent kafka: https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/Producer/Program.cs
            using (var producer = new ProducerBuilder<string, string>(_producerConfing).Build())
            {
                var deliveryReport = await producer.ProduceAsync(topics, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = value });
            }
        }
    }
}