// Arquivo: WebApi/Features/Auth/AuthenticateUserFeature/AuthenticateUserProfile.cs
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature
{
    public sealed class AuthenticateUserProfile : Profile
    {
        public AuthenticateUserProfile()
        {
            // requisição
            CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();

            // resultado
            CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();

            // Mapeamento
            CreateMap<User, AuthenticateUserResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}