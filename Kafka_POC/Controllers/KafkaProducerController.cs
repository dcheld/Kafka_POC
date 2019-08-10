using Kafka_POC.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Kafka_POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaProducerController : ControllerBase
    {
        private readonly KafkaProducer _kafkaProducer;

        public KafkaProducerController(KafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        [HttpPost("{message}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post(string message)
        {
            await _kafkaProducer.ProduceAsync(message);
            return Ok();
        }
    }
}