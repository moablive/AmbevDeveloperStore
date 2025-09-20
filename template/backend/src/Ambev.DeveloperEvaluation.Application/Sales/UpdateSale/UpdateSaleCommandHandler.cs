using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IConfiguration _configuration;
        private readonly IEventProducer _eventProducer;

        public UpdateSaleCommandHandler(
            ISaleRepository saleRepository,
            IConfiguration configuration,
            IEventProducer eventProducer)
        {
            _saleRepository = saleRepository;
            _configuration = configuration; 
            _eventProducer = eventProducer;
        }

        public async Task Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com o ID {request.SaleId} não encontrada.");
            }

            var customerName = _configuration["DemoData:DefaultCustomerName"];
            var branchName = _configuration["DemoData:DefaultBranchName"];
            var unitPrice = _configuration.GetValue<decimal>("DemoData:DefaultProductUnitPrice");
            var descriptionTemplate = _configuration["DemoData:DefaultProductDescriptionTemplate"];

            sale.CustomerId = request.CustomerId;
            sale.BranchId = request.BranchId;
            sale.CustomerName = $"{customerName} (Atualizado)";
            sale.BranchName = $"{branchName} (Atualizado)";   
            sale.SaleDate = System.DateTime.UtcNow;

            var itemsData = request.Items.Select(i => new SaleItemData(i.ProductId, i.Quantity));

            sale.UpdateItemsAndRecalculateTotal(itemsData, unitPrice, descriptionTemplate);

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