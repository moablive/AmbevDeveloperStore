using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales
{
    public class GetAllSalesResult
    {
        public IEnumerable<Sale> Sales { get; set; } = new List<Sale>();
        public int TotalCount { get; set; }
    }
}