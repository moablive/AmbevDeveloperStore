using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Representa a requisição HTTP para criar uma nova venda.
    /// </summary>
    public class CreateSaleRequest
    {
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    /// <summary>
    /// Representa um item da venda na requisição.
    /// </summary>
    public record SaleItemRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}