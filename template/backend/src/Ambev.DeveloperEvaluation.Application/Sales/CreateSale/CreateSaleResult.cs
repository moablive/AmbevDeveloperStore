using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Representa o resultado da operação de criação de Venda.
    /// </summary>
    public class CreateSaleResult
    {
        /// <summary>
        /// O ID da Venda recém-criada.
        /// </summary>
        public Guid SaleId { get; set; }
    }
}