using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            // Mapeia da API para a Camada de Aplicação
            CreateMap<CreateSaleRequest, CreateSaleCommand>();
            CreateMap<SaleItemRequest, SaleItemCommand>();

            // Mapeia da Camada de Aplicação para a API
            CreateMap<CreateSaleResult, CreateSaleResponse>();
        }
    }
}