using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Reflection;
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
    public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration, IWebHostEnvironment env, bool isIntegrationTest = false)
    {
        service.AddScoped<ICacheRepository, CacheRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddSingleton<IJwtFactory, JwtFactory>()
                .AddSingleton<IJwtTokenHandler, JwtTokenHandler>()
                .AddSingleton<ITokenFactory, TokenFactory>()
                .AddSingleton<IJwtTokenValidator, JwtTokenValidator>()
                .AddScoped<SignInManager<AppUser>>();
        service.AddDbContextPool<AppIdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(isIntegrationTest ? "IntegrationTests" : "Default"), o => { o.SetPostgresVersion(18, 0); o.MigrationsAssembly("Web.Api.Infrastructure"); });
            options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            if (isIntegrationTest)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine);
            }
        }).AddDbContextPool<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(isIntegrationTest ? "IntegrationTests" : "Default"), o => { o.SetPostgresVersion(18, 0); o.MigrationsAssembly("Web.Api.Infrastructure"); });
            options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            if (isIntegrationTest)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine);
            }
        });
        if (!isIntegrationTest && env.IsProduction() && !Assembly.GetEntryAssembly().GetName().Name.Equals("GetDocument.Insider")) // XXX: Temporary fix until https://github.com/dotnet/aspnetcore/issues/54698 is fixed
        {
            service.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCache:Connection"];
                options.InstanceName = configuration["RedisCache:InstanceName"];
            });
            service.AddSingleton<IConnectionMultiplexer>(sp =>
                 ConnectionMultiplexer.Connect(new ConfigurationOptions
                 {
                     EndPoints = { configuration["RedisCache:Connection"] },
                     AbortOnConnectFail = false,
                 }));
            var redis = ConnectionMultiplexer.Connect(configuration["RedisCache:Connection"]);
            service.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "AspNetCoreWebApi");
        }
        else
        {
            service.AddDistributedMemoryCache();
            service.AddDataProtection();
        }
        return service;
    }
}