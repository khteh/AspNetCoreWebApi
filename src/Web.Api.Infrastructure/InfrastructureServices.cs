using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration) =>
            service.AddScoped<IUserRepository, UserRepository>()
            .AddSingleton<IJwtFactory, JwtFactory>()
            .AddSingleton<IJwtTokenHandler, JwtTokenHandler>()
            .AddSingleton<ITokenFactory, TokenFactory>()
            .AddSingleton<IJwtTokenValidator, JwtTokenValidator>()
            .AddScoped<SignInManager<AppUser>>()
            .AddDbContext<AppIdentityDbContext>(options => options.UseMySQL(configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")))
            .AddDbContext<AppDbContext>(options => options.UseMySQL(configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
    }
}