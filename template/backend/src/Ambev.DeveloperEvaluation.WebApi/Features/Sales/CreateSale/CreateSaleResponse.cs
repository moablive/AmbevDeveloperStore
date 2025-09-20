using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Representa a resposta da API após a criação de uma venda.
    /// </summary>
    public class CreateSaleResponse
    {
        /// <summary>
        /// O ID da venda que foi criada com sucesso.
        /// </summary>
        public Guid SaleId { get; set; }
    }
}