using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Representa uma Venda no sistema.
    /// </summary>
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public List<SaleItem> Items { get; set; } = new();

        /// <summary>
        /// Atualiza os itens da venda e recalcula o total. Centraliza a lógica de negócio de descontos.
        /// </summary>
        /// <param name="itemsData">Uma coleção de dados dos itens para compor a venda.</param>
        /// <param name="defaultPrice">O preço unitário padrão do produto, vindo da configuração.</param>
        /// <param name="descriptionTemplate">O modelo de descrição do produto, vindo da configuração.</param>
        public void UpdateItemsAndRecalculateTotal(IEnumerable<SaleItemData> itemsData, decimal defaultPrice, string descriptionTemplate)
        {
            Items.Clear();

            foreach (var itemData in itemsData)
            {
                decimal unitPrice = defaultPrice;
                string productDescription = string.Format(descriptionTemplate, itemData.ProductId.ToString().Substring(0, 4));

                decimal discountPercentage = 0;

                // Regras de negócio para desconto
                if (itemData.Quantity >= 4 && itemData.Quantity < 10)
                {
                    discountPercentage = 0.10m; // 10%
                }
                else if (itemData.Quantity >= 10 && itemData.Quantity <= 20)
                {
                    discountPercentage = 0.20m; // 20%
                }

                var totalItemPrice = itemData.Quantity * unitPrice;
                var discountValue = totalItemPrice * discountPercentage;

                Items.Add(new SaleItem
                {
                    ProductId = itemData.ProductId,
                    ProductDescription = productDescription,
                    Quantity = itemData.Quantity,
                    UnitPrice = unitPrice,
                    DiscountApplied = discountValue,
                    TotalAmount = totalItemPrice - discountValue
                });
            }

            TotalAmount = Items.Sum(item => item.TotalAmount);
        }
    }

    /// <summary>
    /// Representa um item dentro de uma Venda.
    /// </summary>
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal TotalAmount { get; set; }
    }
}