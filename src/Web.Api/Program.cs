global using FluentValidation.AspNetCore;
global using MediatR;
global using Microsoft.AspNetCore.Antiforgery;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.CookiePolicy;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Connections;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Serilog;
global using Serilog.Extensions;
global using Serilog.Events;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Net;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Reflection;
using System.Text.Json;
using Web.Api;
using Web.Api.Behaviours;
using Web.Api.Commands;
using Web.Api.Core.Configuration;
using Web.Api.Extensions;
using Web.Api.HealthChecks;
using Web.Api.Helpers;
using Web.Api.Hubs;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Mapping;
using Web.Api.Infrastructure.Helpers;
using Web.Api.Infrastructure.Identity;
using Web.Api.Models.Logging;
using Web.Api.Models.Response;
using Web.Api.Presenters.Grpc;
using Web.Api.Services;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new EcsTextFormatter())
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        ApplicationName = typeof(Program).Assembly.GetName().Name,
        ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
        WebRootPath = "wwwroot",
        Args = args
    });
    IWebHostEnvironment env = builder.Environment;
    string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    bool _isIntegrationTests = !string.IsNullOrEmpty(environment) && environment.Equals("IntegrationTests");
    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                    .AddJsonFile($"appsettings.postgresql.json", true, true)
                    .AddJsonFile($"appsettings.email.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
    //builder.WebHost.UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory())); Changing the host configuration using WebApplicationBuilder.Host is not supported. Use WebApplication.CreateBuilder(WebApplicationOptions) instead.
    int originalMinWorker, originalMinIOC;
    int minWorker = 1000;
    string strMinWorkerThreads = Environment.GetEnvironmentVariable("COMPlus_ThreadPool_ForceMinWorkerThreads");
    if (!string.IsNullOrEmpty(strMinWorkerThreads) && Int32.TryParse(strMinWorkerThreads, out int minWorkerThreads))
        minWorker = minWorkerThreads;
    // Get the current settings.
    ThreadPool.GetMinThreads(out originalMinWorker, out originalMinIOC);
    // Change the minimum number of worker threads to four, but
    // keep the old setting for minimum asynchronous I/O 
    // completion threads.
    if (ThreadPool.SetMinThreads(minWorker, originalMinIOC))
        // The minimum number of threads was set successfully.
        Log.Information($"Using {minWorker} threads");
    else
        // The minimum number of threads was not changed.
        Log.Error($"Failed to set {minWorker} threads. Using original {originalMinWorker} threads");
    string pathBase = Environment.GetEnvironmentVariable("PATH_BASE");
    Log.Information($"Using PathBase: {pathBase}");
    /* https://www.elastic.co/docs/reference/ecs/logging/dotnet/serilog-formatter
     * https://www.nuget.org/packages/Serilog.Enrichers.HttpContext
     * https://github.com/elastic/ecs-dotnet
     * https://github.com/serilog/serilog/wiki/configuration-basics
     */
    builder.Host.UseSerilog((ctx, config) =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
#if false
        if (ctx.HostingEnvironment.IsDevelopment() || ctx.HostingEnvironment.IsStaging())
            config.WriteTo.Async(a => a.Console(LogEventLevel.Verbose, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}"));
        else
            config.WriteTo.Async(a => a.Console(new EcsTextFormatter()));
#endif
    });
    // Add services to the container.
    // Ensure that we make the HttpContextAccessor resolvable through the configuration
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddRazorPages();
    builder.Services.AddOptions();
    builder.Services.Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.ConsentCookieValue = "true";
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });
    // Add framework builder.Services.
    // The following are done in builder.Services.AddInfrastructure()
    //builder.Services.AddDbContextPool<AppIdentityDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
    //builder.Services.AddDbContextPool<AppDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("Web.Api.Infrastructure")));
    // Register the ConfigurationBuilder instance of AuthSettings
    var authSettings = builder.Configuration.GetSection(nameof(AuthSettings));
    builder.Services.Configure<AuthSettings>(authSettings);
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));
    // jwt wire up
    // Get options from app settings
    var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));
    builder.Services.Configure<JwtIssuerOptions>(jwtAppSettingOptions);
    // Configure JwtIssuerOptions
    builder.Services.Configure<JwtIssuerOptions>(options =>
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
    builder.Services.AddAuthentication(options =>
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
                    context.Response.Headers.Append("Token-Expired", "true");
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
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiUser", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess);
        });
    });
    builder.Services.AddAntiforgery(options =>
    {
        // Set Cookie properties using CookieBuilder properties†.
        options.FormFieldName = "AntiforgeryFieldname";
        options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
        options.SuppressXFrameOptionsHeader = false;
    });
    // add identity
    var identityBuilder = builder.Services.AddIdentityCore<AppUser>(o =>
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

    builder.Services.AddTransient<CustomEmailConfirmationTokenProvider<AppUser>>();
    var emailSettings = builder.Configuration.GetSection(nameof(EmailSettings));
    builder.Services.Configure<EmailSettings>(emailSettings);
    builder.Services.AddTransient<IEmailSender, EmailSender>();
    var redisCacheConfig = builder.Configuration.GetSection(nameof(RedisCache));
    builder.Services.Configure<RedisCache>(redisCacheConfig);
    identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
    identityBuilder.AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();//.AddDefaultUI();
    if (!string.IsNullOrEmpty(builder.Configuration["Cors:Domains"]))
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                              {
                                  policy.WithOrigins(builder.Configuration["Cors:Domains"].Split(','))
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                              });
        });
    builder.Services.AddControllersWithViews();
    builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    //.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter())); Fixed in .Net Core 5
    builder.Services.AddAutoMapper(new[] { typeof(IdentityProfile), typeof(GrpcProfile), typeof(ResponseProfile) });
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    builder.Services.AddScoped<IPipelineBehavior<ConfirmEmailCommand, ResponseBase>, LoggingBehavior<ConfirmEmailCommand, ResponseBase>>();
    builder.Services.AddScoped<IPipelineBehavior<ConfirmEmailChangeCommand, ResponseBase>, LoggingBehavior<ConfirmEmailChangeCommand, ResponseBase>>();
    builder.Services.AddScoped<IPipelineBehavior<GenerateChangeEmailTokenCommand, CodeResponse>, LoggingBehavior<GenerateChangeEmailTokenCommand, CodeResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<RegisterUserCommand, RegisterUserResponse>, LoggingBehavior<RegisterUserCommand, RegisterUserResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<LogInCommand, LogInResponse>, LoggingBehavior<LogInCommand, LogInResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>, LoggingBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<GenerateNew2FARecoveryCodesCommand, GenerateNew2FARecoveryCodesResponse>, LoggingBehavior<GenerateNew2FARecoveryCodesCommand, GenerateNew2FARecoveryCodesResponse>>();
    builder.Services.AddEndpointsApiExplorer();
    // Register the Swagger generator, defining 1 or more Swagger documents
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v9.0", new OpenApiInfo
        {
            Title = "ASP.Net Core RESTful, SignalR and GRPC service",
            Version = "v9.0",
            Description = "An ASP.NET Core 9.0 Web API, SignalR and GRPC project to quickly bootstrap new projects.  Includes Identity, JWT authentication w/ refresh tokens.",
            Contact = new OpenApiContact
            {
                Name = "Teh Kok How",
                Email = "funcoolgeek@gmail.com",
                Url = new Uri("https://github.com/khteh/AspNetCoreWebApi"),
            },
        });
        // Swagger 2.+ support
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Description = "Please insert JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
        //{
        //    { "Bearer", new string[] { } }
        //});
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {{ // NOT duplicate. Don't remove.
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }});
        c.CustomSchemaIds(i => i.FullName);
    });
    builder.Services.AddGrpc();
    builder.Services.AddGrpcReflection();
    builder.Services.AddSignalR();
    // Change to use Name as the user identifier for SignalR
    // WARNING: This requires that the source of your JWT token 
    // ensures that the Name claim is unique!
    // If the Name claim isn't unique, users could receive messages 
    // intended for a different user!
    builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    // Change to use email as the user identifier for SignalR
    // builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
    builder.Services.ConfigureApplicationCookie(o =>
    {
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-7.0&tabs=visual-studio#change-email-and-activity-timeout
        o.ExpireTimeSpan = TimeSpan.FromHours(1); // The default inactivity timeout is 14 days.
        o.SlidingExpiration = true;
    });
    // WARNING: use *either* the NameUserIdProvider *or* the 
    // EmailBasedUserIdProvider, but do not use both. 
    // Register Infrastructure Services
    builder.Services.AddInfrastructure(builder.Configuration, env, _isIntegrationTests).AddCore().AddOutputPorts();
    builder.Services.AddHealthChecks()
        .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>() { "live" })
        .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string> { "ready" })
        .AddNpgSql(builder.Configuration["ConnectionStrings:Default"], "SELECT 1;", null, "PostgreSQL Health Check", HealthStatus.Unhealthy, new List<string> { "Database" })
        .AddDbContextCheck<AppDbContext>("AppDbContext", HealthStatus.Unhealthy, new List<string> { "DbContext" });
    builder.Services.AddHostedService<StartupBackgroundService>()
        .AddSingleton<ReadinessHealthCheck>()
        .AddSingleton<LivenessHealthCheck>();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
        options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("10.0.0.0"), 16)); // Load Balancer / VPC Network
    });
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(60);
        //options.ExcludedHosts.Add("example.com");
        //options.ExcludedHosts.Add("www.example.com");
    });
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    });
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.RequestHeaders.Add("sec-ch-ua");
        logging.ResponseHeaders.Add("MyResponseHeader");
        logging.MediaTypeOptions.AddText("application/javascript");
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    });
    if (!string.IsNullOrEmpty(environment) && string.Equals(environment, "Production"))
        builder.Services.AddAllElasticApm();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.MapGrpcReflectionService();
    }
    else
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                var error = context.Features.Get<IExceptionHandlerFeature>();
                if (error != null)
                {
                    context.Response.AddApplicationError(error.Error.Message);
                    await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                }
            }));
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseWhen(
            ctx => ctx.Request.ContentType != "application/grpc",
            builder =>
            {
                builder.UseHttpLogging();
            }
        );
    //app.UseHttpLogging(); https://github.com/dotnet/aspnetcore/issues/39317
    app.UseResponseCaching();
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCookiePolicy(new CookiePolicyOptions() { HttpOnly = HttpOnlyPolicy.Always, Secure = CookieSecurePolicy.Always });
    app.UsePathBase(pathBase);
    app.UseRouting();
    app.UseAuthentication(); // The order in which you register the SignalR and ASP.NET Core authentication middleware matters. Always call UseAuthentication before UseSignalR so that SignalR has a user on the HttpContext.
    app.UseAuthorization();
    app.UseWebSockets();
    //if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    app.UseSwagger().UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"{pathBase}/swagger/v9.0/swagger.json", "AspNetCoreWebApi V9.0");
    });
    app.UseSerilogRequestLogging();
    app.UseSerilogMemoryUsageExact();
    app.Use(async (context, next) =>
    {
        /* Request method, scheme, and path
         * _logger.LogInformation($"Method: {context.Request.Method}, Scheme: {context.Request.Scheme}, PathBase: {context.Request.PathBase}, Path: {context.Request.Path}, IP: {context.Connection.RemoteIpAddress}, Host: {context.Request.Host}, ContentLength: {context.Request.ContentLength}");
         * The following should have been handled by Serilog.Enrichers.HttpContext configured in appsettings.json
         * https://www.nuget.org/packages/Serilog.Enrichers.HttpContext : outputTemplate: "[{Timestamp:HH:mm:ss}] {Level:u3} ClientIP: {ClientIp} CorrelationId: {CorrelationId} header-name: {headername} {Message:lj}{NewLine}{Exception}"
         * https://github.com/elastic/ecs-dotnet
         */
        RequestLog requestLog = new RequestLog(context?.Request?.Method,
                                            context?.Request?.Scheme,
                                            context?.Request?.Protocol,
                                            context?.Request?.PathBase,
                                            context?.Request?.Path,
                                            context?.Request?.Host.ToString()
                                            /*context?.Request?.ContentLength,
                                            context?.Connection?.RemoteIpAddress?.ToString(), Use Serilog.Enrichers.HttpContext.WithClientIp
                                            context?.Request?.QueryString.ToString(), WithRequestQuery 
                                            context?.Request?.ContentType,*/
                                            );
        app.Logger.LogInformation(JsonSerializer.Serialize(requestLog)); // geoip works with JSON format
        // Headers
        //foreach (var header in context.Request.Headers)
        //    _logger.LogInformation("Header: {KEY}: {VALUE}", header.Key, header.Value);
        // Connection: RemoteIp
        context.Request.PathBase = new PathString(pathBase); // Kubernetes ingress rule.
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        // https://dotnetthoughts.net/implementing-content-security-policy-in-aspnetcore/
        context.Response.Headers.Append("Content-Security-Policy", "script-src 'self' 'unsafe-inline' cdn.jsdelivr.net cdnjs.cloudflare.com;");
        context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(10)
        };
        context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
        if (string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
        {
            // The request token can be sent as a JavaScript-readable cookie, 
            // and Angular uses it by default.
            IAntiforgery antiforgery = app.Services.GetRequiredService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions() { HttpOnly = false, Secure = true });
        }
        await next(context);
    });
    app.MapRazorPages();
    app.MapControllers();
    app.MapHub<ChatHub>($"/chatHub", o => o.Transports = HttpTransportType.WebSockets);
    app.MapGrpcService<AccountsService>();
    app.MapGrpcService<AuthService>();
    app.MapGrpcService<PingService>();
    app.MapHealthChecks("/health/live", new HealthCheckOptions()
    {
        AllowCachingResponses = false,
        Predicate = healthCheck => healthCheck.Tags.Contains("live") || healthCheck.Tags.Contains("Database") || healthCheck.Tags.Contains("DbContext")
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions()
    {
        AllowCachingResponses = false,
        Predicate = healthCheck => healthCheck.Tags.Contains("ready") || healthCheck.Tags.Contains("Database") || healthCheck.Tags.Contains("DbContext")
    });
    IHostApplicationLifetime lifetime = app.Lifetime;
    ReadinessHealthCheck readinessHealthCheck = app.Services.GetRequiredService<ReadinessHealthCheck>();
    lifetime.ApplicationStarted.Register(() => AppStarted(app.Logger, readinessHealthCheck));
    lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation($"{typeof(Program).Assembly.GetName().Name} stopping..."));
    lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation($"{typeof(Program).Assembly.GetName().Name} stopped!"));
    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "HostAbortedException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal($"Exception: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}
static void AppStarted(Microsoft.Extensions.Logging.ILogger logger, ReadinessHealthCheck readinessHealthCheck)
{
    logger.LogInformation($"{typeof(Program).Assembly.GetName().Name} started!");
    readinessHealthCheck.StartupCompleted = true;
}
public partial class Program { } // so you can reference it from tests