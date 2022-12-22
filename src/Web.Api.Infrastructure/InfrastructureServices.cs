using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Web.Api.Core.Configuration;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
namespace Microsoft.Extensions.DependencyInjection;
public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration, IWebHostEnvironment env, bool isIntegrationTest = false)
    {
        service.AddScoped<IUserRepository, UserRepository>()
                .AddSingleton<IJwtFactory, JwtFactory>()
                .AddSingleton<IJwtTokenHandler, JwtTokenHandler>()
                .AddSingleton<ITokenFactory, TokenFactory>()
                .AddSingleton<IJwtTokenValidator, JwtTokenValidator>()
                .AddScoped<SignInManager<AppUser>>();
        service.AddDbContextPool<AppIdentityDbContext>(options => options.UseNpgsql(configuration.GetConnectionString(isIntegrationTest ? "IntegrationTests" : "Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")))
                    .AddDbContextPool<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString(isIntegrationTest ? "IntegrationTests" : "Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
        if (!isIntegrationTest && env.IsProduction())
        {
            service.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCache:Connection"];
                options.InstanceName = configuration["RedisCache:InstanceName"];
            });
            var redis = ConnectionMultiplexer.Connect(configuration["RedisCache:Connection"]);
            service.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }
        return service;
    }
}