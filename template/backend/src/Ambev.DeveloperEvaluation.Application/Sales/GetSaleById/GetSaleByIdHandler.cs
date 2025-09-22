using Ambev.DeveloperEvaluation.Application.Common.Interfaces; // ADICIONADO
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById
{
    public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, GetSaleByIdResult>
    {
        private readonly ISaleReadRepository _saleReadRepository;
        private readonly IMapper _mapper;

        public GetSaleByIdHandler(ISaleReadRepository saleReadRepository, IMapper mapper)
        {
            _saleReadRepository = saleReadRepository;
            _mapper = mapper;
        }

        public async Task<GetSaleByIdResult> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var sale = await _saleReadRepository.GetByIdAsync(request.SaleId);

            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com o ID {request.SaleId} não encontrada.");
            }

            return _mapper.Map<GetSaleByIdResult>(sale);
        }
    }
}