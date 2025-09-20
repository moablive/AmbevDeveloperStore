using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById
{
    public class GetSaleByIdProfile : Profile
    {
        public GetSaleByIdProfile()
        {
            CreateMap<GetSaleByIdResult, GetSaleByIdResponse>();
            CreateMap<SaleItemResult, SaleItemResponse>();
        }
    }
}