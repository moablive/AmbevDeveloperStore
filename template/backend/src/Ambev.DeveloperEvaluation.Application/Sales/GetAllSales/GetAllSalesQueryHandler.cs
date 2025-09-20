using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales
{
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, GetAllSalesResult>
    {
        private readonly ISaleRepository _saleRepository;

        public GetAllSalesQueryHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<GetAllSalesResult> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var (sales, totalCount) = await _saleRepository.GetAllAsync(request.PageNumber, request.PageSize, request.OrderBy);

            return new GetAllSalesResult
            {
                Sales = sales,
                TotalCount = totalCount
            };
        }
    }
}