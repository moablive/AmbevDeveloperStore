using MediatR;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Comando para criar uma nova Venda.
    /// Contém todas as informações necessárias para registrar a venda e seus itens.
    /// </summary>
    public class CreateSaleCommand : IRequest<CreateSaleResult>
    {
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public List<SaleItemCommand> Items { get; set; } = new();
    }

    /// <summary>
    /// Representa um item dentro do comando de criação de Venda.
    /// </summary>
    public record SaleItemCommand
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}