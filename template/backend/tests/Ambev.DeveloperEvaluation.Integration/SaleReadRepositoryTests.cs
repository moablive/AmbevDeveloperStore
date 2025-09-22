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
            _configuration = Substitute.For<IConfiguration>();

            _configuration["DemoData:DefaultCustomerName"].Returns("Cliente Teste");
            _configuration["DemoData:DefaultBranchName"].Returns("Filial Teste");

            var unitPriceSection = Substitute.For<IConfigurationSection>();
            unitPriceSection.Value.Returns("100.0");
            _configuration.GetSection("DemoData:DefaultProductUnitPrice").Returns(unitPriceSection);

            _configuration["DemoData:DefaultProductDescriptionTemplate"].Returns("Produto Teste {0}");
            _configuration["Kafka:SalesTopic"].Returns("sales-test-topic");

            _handler = new CreateSaleHandler(_saleRepository, _eventProducer, _configuration);
        }

        [Fact]
        public async Task Handle_WhenQuantityIs5_ShouldApply10PercentDiscount()
        {
            var command = new CreateSaleCommand { Items = new List<SaleItemCommand> { new SaleItemCommand { Quantity = 5 } } };
            Sale savedSale = null;
            _saleRepository.When(repo => repo.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()))
                         .Do(callInfo => savedSale = callInfo.Arg<Sale>());

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
            _saleRepository.When(repo => repo.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()))
                         .Do(callInfo => savedSale = callInfo.Arg<Sale>());

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
            _saleRepository.When(repo => repo.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()))
                         .Do(callInfo => savedSale = callInfo.Arg<Sale>());

            await _handler.Handle(command, CancellationToken.None);

            savedSale.Should().NotBeNull();
            var savedItem = savedSale.Items.First();
            savedItem.DiscountApplied.Should().Be(0m);
            savedItem.TotalAmount.Should().Be(300.0m);
        }
    }
}