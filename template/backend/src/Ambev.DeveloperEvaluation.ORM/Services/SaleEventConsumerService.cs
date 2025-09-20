using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Confluent.Kafka;

namespace Ambev.DeveloperEvaluation.ORM.Services
{
    public class SaleEventConsumerService : BackgroundService
    {
        private readonly ILogger<SaleEventConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public SaleEventConsumerService(
            ILogger<SaleEventConsumerService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bootstrap = _configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
            var topic = _configuration["Kafka:Topic"] ?? "sales-events";

            _logger.LogInformation("Consumidor de eventos de venda iniciando. BootstrapServers={Bootstrap}, Topic={Topic}",
                bootstrap, topic);

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = "sales-processor-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true,
                EnablePartitionEof = false,
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetErrorHandler((_, e) =>
                {
                    _logger.LogWarning("Kafka client error: {Reason} (Code={Code}, IsFatal={IsFatal})",
                        e.Reason, e.Code, e.IsFatal);
                })
                .Build();

            using var registration = stoppingToken.Register(() =>
            {
                try { consumer.Close(); } catch {}
            });

            try
            {
                consumer.Subscribe(topic);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Falha ao assinar o tópico {Topic}", topic);
                throw;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    if (cr is null || cr.Message is null)
                        continue;

                    _logger.LogInformation("Evento de venda recebido na partição {Partition} offset {Offset}.",
                        cr.Partition.Value, cr.Offset.Value);

                    Sale? saleEvent = null;
                    try
                    {
                        saleEvent = JsonSerializer.Deserialize<Sale>(cr.Message.Value);
                    }
                    catch (Exception jsonEx)
                    {
                        _logger.LogError(jsonEx, "Falha ao desserializar mensagem: {Payload}", cr.Message.Value);
                        continue;
                    }

                    if (saleEvent is not null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var saleReadRepository = scope.ServiceProvider.GetRequiredService<ISaleReadRepository>();
                        await saleReadRepository.UpsertAsync(saleEvent);
                        _logger.LogInformation("Venda {SaleId} salva/atualizada no read model (MongoDB).", saleEvent.Id);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Erro no consume: {Reason} (IsFatal={IsFatal}, Code={Code})",
                        ex.Error.Reason, ex.Error.IsFatal, ex.Error.Code);

                    if (ex.Error.IsFatal)
                        throw;

                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar evento de venda.");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            try
            {
                consumer.Close();
            }
            catch 
            { 
            }

            _logger.LogInformation("Consumidor de eventos de venda finalizando.");
        }
    }
}
