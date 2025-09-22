using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales
{
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, GetAllSalesResult>
    {
        private readonly ISaleReadRepository _saleReadRepository;

        public GetAllSalesQueryHandler(ISaleReadRepository saleReadRepository)
        {
            _saleReadRepository = saleReadRepository;
        }

        public async Task<GetAllSalesResult> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            // 'OrderBy' => no Read Model.
            var (sales, totalCount) = await _saleReadRepository.GetAllAsync(request.PageNumber, request.PageSize);

            return new GetAllSalesResult
            {
                Sales = sales,
                TotalCount = totalCount
            };
        }
    }
}