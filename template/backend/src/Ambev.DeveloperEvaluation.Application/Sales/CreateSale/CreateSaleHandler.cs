using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Handler para o comando de criação de Venda.
    /// </summary>
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IEventProducer _eventProducer;
        private readonly IConfiguration _configuration;

        public CreateSaleHandler(
            ISaleRepository saleRepository,
            IEventProducer eventProducer,
            IConfiguration configuration)
        {
            _saleRepository = saleRepository;
            _eventProducer = eventProducer;
            _configuration = configuration; 
        }

        public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var customerName = _configuration["DemoData:DefaultCustomerName"];
            var branchName = _configuration["DemoData:DefaultBranchName"];
            var unitPrice = _configuration.GetValue<decimal>("DemoData:DefaultProductUnitPrice");
            var descriptionTemplate = _configuration["DemoData:DefaultProductDescriptionTemplate"];

            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                SaleDate = DateTime.UtcNow,
                CustomerId = request.CustomerId,
                CustomerName = customerName, // ALTERADO: Usa o nome do JSON
                BranchId = request.BranchId,
                BranchName = branchName,     // ALTERADO: Usa o nome do JSON
                IsCancelled = false
            };

            var itemsData = request.Items.Select(i => new SaleItemData(i.ProductId, i.Quantity));

            sale.UpdateItemsAndRecalculateTotal(itemsData, unitPrice, descriptionTemplate);

            await _saleRepository.AddAsync(sale, cancellationToken);

            var topicName = _configuration["Kafka:SalesTopic"];
            if (string.IsNullOrEmpty(topicName))
            {
                throw new InvalidOperationException("O tópico do Kafka não está configurado em appsettings.json (Kafka:SalesTopic)");
            }

            await _eventProducer.ProduceAsync(topicName, sale);

            return new CreateSaleResult { SaleId = sale.Id };
        }
    }
}