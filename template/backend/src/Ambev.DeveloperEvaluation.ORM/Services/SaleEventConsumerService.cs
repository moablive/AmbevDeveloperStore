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
            // --- Leitura da configuração ---
            var bootstrap = _configuration["Kafka:BootstrapServers"];
            var topic = _configuration["Kafka:SalesTopic"];

            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(bootstrap))
            {
                _logger.LogCritical("Configurações do Kafka (BootstrapServers ou SalesTopic) não encontradas.");
                return;
            }

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

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(topic);

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
            _logger.LogInformation("Consumidor de eventos de venda finalizando.");
        }
    }
}