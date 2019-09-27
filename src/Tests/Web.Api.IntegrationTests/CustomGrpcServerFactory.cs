using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.TestHost;
using Grpc.Net.ClientFactory;
using Grpc.Net.ClientFactory.Internal;
using Web.Api.Core.Accounts;
using Web.Api.Core.Auth;
namespace Web.Api.IntegrationTests
{
    public class CustomGrpcServerFactory<TStartup> : WebApplicationFactory<Startup>
    {
        public ServiceProvider ServiceProvider {get; private set;}
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase().AddLogging()
                    .BuildServiceProvider();
                services.AddGrpcClient<Accounts.AccountsClient>(o => { o.Address = new Uri("http://localhost");})
                    .EnableCallContextPropagation();
                    //.AddInterceptor(() => new LoggingInterceptor());
                    //.AddHttpMessageHandler(() => ClientTestHelpers.CreateTestMessageHandler(new HelloReply()));
                services.AddGrpcClient<Auth.AuthClient>(o => { o.Address = new Uri("http://localhost");})
                    .EnableCallContextPropagation();
                    //.AddInterceptor(() => new LoggingInterceptor());

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                services.AddDbContext<AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryIdentityDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                services.AddScoped<SignInManager<AppUser>>();
                services.AddScoped<ILogger<UserRepository>>(provider => {
                    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return loggerFactory.CreateLogger<UserRepository>();
                });
                services.AddDistributedMemoryCache();
                // Build the service provider.
                ServiceProvider sp = services.BuildServiceProvider();
                ServiceProvider = sp;
                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<AppDbContext>();
                    var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomGrpcServerFactory<TStartup>>>();

                    // Ensure the database is created.
                    appDb.Database.EnsureCreated();
                    identityDb.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        SeedData.PopulateTestData(identityDb);
                        SeedData.PopulateTestData(appDb);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}