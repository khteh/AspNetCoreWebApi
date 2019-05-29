using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Interfaces;
using Web.Api.Infrastructure.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service) =>
            service.AddScoped<IUserRepository, UserRepository>()
            .AddSingleton<IJwtFactory, JwtFactory>()
            .AddSingleton<IJwtTokenHandler, JwtTokenHandler>()
            .AddSingleton<ITokenFactory, TokenFactory>()
            .AddSingleton<IJwtTokenValidator, JwtTokenValidator>();
    }
}