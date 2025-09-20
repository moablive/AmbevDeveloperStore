using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale; 

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleRequest
    {
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public List<SaleItemRequest> Items { get; set; } = new();
    }
}