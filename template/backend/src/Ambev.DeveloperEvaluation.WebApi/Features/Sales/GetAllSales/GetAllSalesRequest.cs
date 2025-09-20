using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales
{
    public class GetAllSalesRequest
    {
        [FromQuery(Name = "_page")]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "_size")]
        public int PageSize { get; set; } = 10;
    }
}