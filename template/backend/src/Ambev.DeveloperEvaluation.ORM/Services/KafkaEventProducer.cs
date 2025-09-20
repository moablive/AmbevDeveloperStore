using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.ORM.Services
{
    public class KafkaEventProducer : IEventProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaEventProducer> _logger;

        public KafkaEventProducer(IConfiguration configuration, ILogger<KafkaEventProducer> logger) 
        {
            _logger = logger; 
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            try
            {
                var deliveryReport = await _producer.ProduceAsync(topic, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = jsonMessage });

                _logger.LogInformation("Mensagem entregue ao tópico {Topic} - Partição {Partition} - Offset {Offset}",
                    deliveryReport.Topic, deliveryReport.Partition, deliveryReport.Offset);
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError(e, "Falha ao entregar mensagem: {Reason}", e.Error.Reason);
                throw;
            }
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}