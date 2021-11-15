using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Interfaces;
namespace Microsoft.Extensions.DependencyInjection;
public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration, bool useInMemoryDatabase = false)
    {
        service.AddScoped<IUserRepository, UserRepository>()
                .AddSingleton<IJwtFactory, JwtFactory>()
                .AddSingleton<IJwtTokenHandler, JwtTokenHandler>()
                .AddSingleton<ITokenFactory, TokenFactory>()
                .AddSingleton<IJwtTokenValidator, JwtTokenValidator>()
                .AddScoped<SignInManager<AppUser>>();
        if (!useInMemoryDatabase)
            service.AddDbContextPool<AppIdentityDbContext>(options => options.UseMySql(configuration.GetConnectionString("Default"), ServerVersion.AutoDetect(configuration.GetConnectionString("Default")), b => b.MigrationsAssembly("Web.Api.Infrastructure")))
                    .AddDbContextPool<AppDbContext>(options => options.UseMySql(configuration.GetConnectionString("Default"), ServerVersion.AutoDetect(configuration.GetConnectionString("Default")), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
        return service;
    }
}