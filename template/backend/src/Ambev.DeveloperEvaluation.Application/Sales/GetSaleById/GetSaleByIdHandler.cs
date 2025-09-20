using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById
{
    public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, GetSaleByIdResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        // (repositório e mapper)
        public GetSaleByIdHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<GetSaleByIdResult> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Busca a venda no banco de dados usando o ID da requisição
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);

            // 2. Se a venda não for encontrada, lança uma exceção
            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com o ID {request.SaleId} não encontrada.");
            }

            // 3. Se encontrou, usa o AutoMapper
            return _mapper.Map<GetSaleByIdResult>(sale);
        }
    }
}