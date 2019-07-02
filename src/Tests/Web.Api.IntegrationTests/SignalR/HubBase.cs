using System.Linq;
using System;
using AutoMapper;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Encodings.Web;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data.Mapping;
using Web.Api.Infrastructure.Helpers;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.Api;
using Web.Api.Hubs;
using Microsoft.AspNetCore.Http.Connections;

namespace Biz4x.Frontend.Web.Integration.Test.SignalR
{
    public abstract class HubBase<THub> : WebApplicationFactory<Startup> where THub : Hub
    {
        protected TestServer TestServer {get; private set;}
        protected HttpClient HttpClient { get => CreateClient(); }
        public HubBase(string url)
        {
            var contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddJsonFile($"appsettings.local.json", true, true)
                .AddEnvironmentVariables().Build();
            IConfigurationSection jwtAppSettingOptions = config.GetSection(nameof(JwtIssuerOptions));
            var authSettings = config.GetSection(nameof(AuthSettings));
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                            logging.AddConsole();
                            logging.AddDebug();
                            logging.AddEventSourceLogger();
                            logging.AddSerilog(dispose: true);
                        })
                        .ConfigureServices(services => {
                            services.AddAutoMapper(typeof(DataProfile));
                            services.AddSingleton(UrlEncoder.Default);
                            services.AddSignalR();
                            // Configure JwtIssuerOptions
                            services.Configure<AuthSettings>(authSettings);
                            services.Configure<JwtIssuerOptions>(options =>
                            {
                                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
                            });
                            var tokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                                ValidateAudience = true,
                                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = signingKey,

                                RequireExpirationTime = true,//false,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero
                            };
                            services.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            }).AddJwtBearer(configureOptions =>
                            {
                                configureOptions.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                                configureOptions.TokenValidationParameters = tokenValidationParameters;
                                configureOptions.SaveToken = true;
                                configureOptions.Events = new JwtBearerEvents
                                    {
                                        OnAuthenticationFailed = context =>
                                        {
                                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                                context.Response.Headers.Add("Token-Expired", "true");
                                            return Task.CompletedTask;
                                        },
                                        // We have to hook the OnMessageReceived event in order to
                                        // allow the JWT authentication handler to read the access
                                        // token from the query string when a WebSocket or 
                                        // Server-Sent Events request comes in.
                                        OnMessageReceived = async (context) =>
                                        {
                                            if (context.Request.Path.StartsWithSegments("/chatHub"))
                                            {
                                                string accessToken = context.Request.Query["access_token"];
                                                if (string.IsNullOrEmpty(accessToken) && context.Request.Headers.ContainsKey("Authorization"))
                                                {
                                                    accessToken = context.Request.Headers["Authorization"];
                                                    accessToken = accessToken.Split(" ")[1];
                                                }
                                                if (!string.IsNullOrEmpty(accessToken))
                                                    context.Token = accessToken;
                                            }
                                        }
                                    };
                            });
                            services.AddAuthorization(options =>
                            {
                                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
                            });
                        })
                            //.Configure(app => app.UseSignalR(configure => configure.MapHub<BoardRateHub>("/booking", options => options.Transports = HttpTransportType.WebSockets)));
                        .Configure(app => {
                            app.UseWebSockets();
                            app.UseAuthentication(); // The order in which you register the SignalR and ASP.NET Core authentication middleware matters. Always call UseAuthentication before UseSignalR so that SignalR has a user on the HttpContext.
                            //app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub", options => options.Transports = HttpTransportType.WebSockets));
                            app.UseSignalR(configure => configure.MapHub<THub>(url));
                        });
            ConfigureWebHost(webHostBuilder);
            TestServer = new TestServer(webHostBuilder);
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

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

                services.AddDistributedMemoryCache();
                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<AppDbContext>();
                    var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();

                    var logger = scopedServices.GetRequiredService<ILogger<HubBase<THub>>>();

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