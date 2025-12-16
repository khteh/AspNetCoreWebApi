using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Web.Api.Infrastructure.Email;
using Web.Api.Infrastructure.Identity;
/* This is used by EF migration. Do NOT remove!
 * https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
 */
namespace Web.Api.Infrastructure.Data;
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 1. Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args).Build();
        // 2. Retrieve the connection string
        string? connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'Default' not found.");
        ServiceCollection services = new ServiceCollection();
        AddIdentity(services);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        DbContextOptionsBuilder<AppDbContext> builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseApplicationServiceProvider(serviceProvider);
        builder.UseNpgsql(connectionString, o => { o.SetPostgresVersion(18, 0); o.MigrationsAssembly("Web.Api.Infrastructure"); });
        builder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
            builder.LogTo(Console.WriteLine);
        return new AppDbContext(builder.Options);
    }
    public static IServiceCollection AddIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>(o =>
        {
            o.Stores.MaxLengthForKeys = 128;
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 6;
            o.Tokens.ProviderMap.Add("IAMEmailConfirmation", new TokenProviderDescriptor(typeof(CustomEmailConfirmationTokenProvider<AppUser>)));
            o.Tokens.EmailConfirmationTokenProvider = "IAMEmailConfirmation";
        });
        return services;
    }
}