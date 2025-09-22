using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IEventProducer _eventProducer;     
        private readonly IConfiguration _configuration;   

        public CancelSaleCommandHandler(
            ISaleRepository saleRepository,
            IEventProducer eventProducer,
            IConfiguration configuration)
        {
            _saleRepository = saleRepository;
            _eventProducer = eventProducer;
            _configuration = configuration;
        }

        public async Task Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            // Busca a venda no banco de dados
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);

            // Se não encontrar, lança exceção para o controller retornar 404
            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com o ID {request.SaleId} não encontrada.");
            }

            if (sale.IsCancelled)
            {
                return;
            }

            sale.IsCancelled = true;

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            var topicName = _configuration["Kafka:SalesTopic"];
            if (string.IsNullOrEmpty(topicName))
            {
                throw new InvalidOperationException("O tópico do Kafka não está configurado (Kafka:SalesTopic)");
            }

            await _eventProducer.ProduceAsync(topicName, sale);
        }
    }
}