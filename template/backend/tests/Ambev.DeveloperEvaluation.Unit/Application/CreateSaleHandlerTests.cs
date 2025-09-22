using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class CreateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IEventProducer _eventProducer;
        private readonly IConfiguration _configuration;
        private readonly CreateSaleHandler _handler;

        public CreateSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _eventProducer = Substitute.For<IEventProducer>();

            //  memória
            var inMemorySettings = new Dictionary<string, string>
            {
                { "DemoData:DefaultCustomerName", "Cliente Teste" },
                { "DemoData:DefaultBranchName", "Filial Teste" },
                { "DemoData:DefaultProductUnitPrice", "100.0" },
                { "DemoData:DefaultProductDescriptionTemplate", "Produto Teste {0}" },
                { "Kafka:SalesTopic", "sales-test-topic" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _eventProducer.ProduceAsync(Arg.Any<string>(), Arg.Any<Sale>()).Returns(Task.CompletedTask);

            _handler = new CreateSaleHandler(_saleRepository, _eventProducer, _configuration);
        }

        [Fact]
        public async Task Handle_WhenQuantityIs5_ShouldApply10PercentDiscount()
        {
            var command = new CreateSaleCommand { Items = new List<SaleItemCommand> { new SaleItemCommand { Quantity = 5 } } };
            Sale savedSale = null;

            _saleRepository
                .AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    savedSale = callInfo.Arg<Sale>();
                    return Task.CompletedTask;  
                });

            await _handler.Handle(command, CancellationToken.None);

            savedSale.Should().NotBeNull();
            var savedItem = savedSale.Items.First();
            savedItem.DiscountApplied.Should().Be(50.0m);
            savedItem.TotalAmount.Should().Be(450.0m);
        }

        [Fact]
        public async Task Handle_WhenQuantityIs12_ShouldApply20PercentDiscount()
        {
            var command = new CreateSaleCommand { Items = new List<SaleItemCommand> { new SaleItemCommand { Quantity = 12 } } };
            Sale savedSale = null;
            _saleRepository
                .AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    savedSale = callInfo.Arg<Sale>();
                    return Task.CompletedTask;
                });

            await _handler.Handle(command, CancellationToken.None);

            savedSale.Should().NotBeNull();
            var savedItem = savedSale.Items.First();
            savedItem.DiscountApplied.Should().Be(240.0m);
            savedItem.TotalAmount.Should().Be(960.0m);
        }

        [Fact]
        public async Task Handle_WhenQuantityIs3_ShouldApplyNoDiscount()
        {
            var command = new CreateSaleCommand { Items = new List<SaleItemCommand> { new SaleItemCommand { Quantity = 3 } } };
            Sale savedSale = null;
            _saleRepository
                .AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    savedSale = callInfo.Arg<Sale>();
                    return Task.CompletedTask;
                });

            await _handler.Handle(command, CancellationToken.None);

            savedSale.Should().NotBeNull();
            var savedItem = savedSale.Items.First();
            savedItem.DiscountApplied.Should().Be(0m);
            savedItem.TotalAmount.Should().Be(300.0m);
        }
    }
}