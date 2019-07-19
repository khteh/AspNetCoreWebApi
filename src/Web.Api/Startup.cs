using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web.Api.Extensions;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data.Mapping;
using Web.Api.Infrastructure.Helpers;
using Web.Api.Infrastructure.Identity;
using Web.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Connections;
using Web.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Web.Api.Infrastructure.Data;

namespace Web.Api
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        { 
            _logger = logger;
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add framework services.
            // The following are done in services.AddInfrastructure()
            //services.AddDbContext<AppIdentityDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
            //services.AddDbContext<AppDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));

            // Register the ConfigurationBuilder instance of AuthSettings
            var authSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
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

                RequireExpirationTime = true,
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
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
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

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            // add identity
            var identityBuilder = services.AddIdentityCore<AppUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });

            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddAutoMapper(typeof(DataProfile));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new Info { Title = "AspNetCoreApiStarter", Version = "v2" });
                // Swagger 2.+ support
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
            services.AddSignalR();
            // Change to use Name as the user identifier for SignalR
            // WARNING: This requires that the source of your JWT token 
            // ensures that the Name claim is unique!
            // If the Name claim isn't unique, users could receive messages 
            // intended for a different user!
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            // Change to use email as the user identifier for SignalR
            // services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

            // WARNING: use *either* the NameUserIdProvider *or* the 
            // EmailBasedUserIdProvider, but do not use both. 
            // Register Infrastructure Services
            services.AddInfrastructure(Configuration).AddCore().AddOutputPorts();//.AddHealthCheck();
            services.AddHealthChecks()
                .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>(){"Liveness"})
                .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string>{ "Readiness" })
                .AddMySql(Configuration["ConnectionStrings:DefaultConnection"])
                .AddDbContextCheck<AppDbContext>();
            services.AddHostedService<StartupHostedService>()
                .AddSingleton<ReadinessHealthCheck>()
                .AddSingleton<LivenessHealthCheck>();

            //services.AddScoped<AuthController>();
            //ServiceProvider provider = services.BuildServiceProvider();
            //Web.Api.Core.Interfaces.Services.ILogger logger = provider.GetRequiredService<Web.Api.Core.Interfaces.Services.ILogger>();
            //IJwtTokenHandler jwtTokenHandler = provider.GetRequiredService<IJwtTokenHandler>();
            //IJwtFactory jwtFactory = provider.GetRequiredService<IJwtFactory>();
            //AuthController auth = provider.GetRequiredService<AuthController>();
            //return provider;
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string pathBase = env.IsDevelopment() ? string.Empty : "/apistarter";
            app.Use(async (context, next) =>
            {
                // Request method, scheme, and path
                _logger.LogInformation($"Method: {context.Request.Method}, Scheme: {context.Request.Scheme}, PathBase: {context.Request.PathBase}, Path: {context.Request.Path}, IP: {context.Connection.RemoteIpAddress}, Host: {context.Request.Host}, ContentLength: {context.Request.ContentLength}");
                // Headers
                //foreach (var header in context.Request.Headers)
                //    _logger.LogInformation("Header: {KEY}: {VALUE}", header.Key, header.Value);
                // Connection: RemoteIp
                context.Request.PathBase = new PathString(pathBase); // Kubernetes ingress rule
                await next();
            });
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-2.1
            #if false
            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString(pathBase);
                return next();
            });
            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments(pathBase, out var remainder))
                    context.Request.Path = remainder;
                return next();
            });
            #endif
            //app.UsePathBase(pathBase);
            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        });
                });
            app.UseHealthChecks($"/health/live", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "Liveness"
            })
            .UseHealthChecks($"/health/ready", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "Readiness",
            });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{pathBase}/swagger/v2/swagger.json", "AspNetCoreApiStarter V2");
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseWebSockets();
            app.UseAuthentication(); // The order in which you register the SignalR and ASP.NET Core authentication middleware matters. Always call UseAuthentication before UseSignalR so that SignalR has a user on the HttpContext.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalR(routes => routes.MapHub<ChatHub>("/chatHub", options => options.Transports = HttpTransportType.WebSockets));
            app.UseMvc();
        }
    }
}