using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka_POC.Service
{
    public class Worker : BackgroundService
    {
        private readonly KafkaConsumer _kafkaConsumer;

        public Worker(KafkaConsumer kafkaConsumer)
        {
            _kafkaConsumer = kafkaConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _kafkaConsumer.ConsumeAsync(stoppingToken);
        }
    }
}