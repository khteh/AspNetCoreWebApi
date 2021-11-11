using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Api;
using Web.Api.Behaviours;
using Web.Api.Commands;
using Web.Api.Extensions;
using Web.Api.HealthChecks;
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

var builder = WebApplication.CreateBuilder(args);
#if false
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = "wwwroot"
});
#endif
IWebHostEnvironment env = builder.Environment;
string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
bool _isIntegrationTests = !string.IsNullOrEmpty(environment) && environment.Equals("IntegrationTests");
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddJsonFile($"appsettings.mysql.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
//builder.WebHost.UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory())); Changing the host configuration using WebApplicationBuilder.Host is not supported. Use WebApplication.CreateBuilder(WebApplicationOptions) instead.
LoggerConfiguration logConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration);
//.MinimumLevel.Debug()
//.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//.WriteTo.RollingFile(config["Logging:LogFile"], fileSizeLimitBytes: 10485760, retainedFileCountLimit: null)
//.Enrich.FromLogContext();
if (env.IsDevelopment())
    //config.WriteTo.Console(new CompactJsonFormatter())
    logConfig.WriteTo.Console(LogEventLevel.Verbose, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
else
    logConfig.WriteTo.Console(new ElasticsearchJsonFormatter());
// Create the logger
Log.Logger = logConfig.CreateLogger();
try
{
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
    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
}
catch (Exception e)
{
    Log.Fatal($"Exception: {e.Message}");
}
finally
{
    Log.CloseAndFlush();
}
builder.WebHost.UseSerilog((ctx, config) =>
                     {
                         config.ReadFrom.Configuration(ctx.Configuration);
                         if (ctx.HostingEnvironment.IsDevelopment())
                             config.WriteTo.Console(LogEventLevel.Verbose, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
                     });
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOptions();
builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
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
// api user claim policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
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
    // configure identity options
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 6;
});
identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
identityBuilder.AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews().AddFluentValidation();
//.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter())); Fixed in .Net Core 5
builder.Services.AddAutoMapper(new[] { typeof(IdentityProfile), typeof(GrpcProfile), typeof(ResponseProfile) });
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddScoped<IPipelineBehavior<RegisterUserCommand, RegisterUserResponse>, LoggingBehavior<RegisterUserCommand, RegisterUserResponse>>();
builder.Services.AddScoped<IPipelineBehavior<LoginCommand, LoginResponse>, LoggingBehavior<LoginCommand, LoginResponse>>();
builder.Services.AddScoped<IPipelineBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>, LoggingBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>>();
builder.Services.AddEndpointsApiExplorer();
// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v3", new OpenApiInfo
    {
        Title = "ASP.Net Core RESTful, SignalR and GRPC service",
        Version = "v3",
        Description = "An ASP.NET Core 3.0 Web API and GRPC project to quickly bootstrap new projects.  Includes Identity, JWT authentication w/ refresh tokens.",
        Contact = new OpenApiContact
        {
            Name = "Teh Kok How",
            Email = "funcoolgeek@gmail.com",
            Url = new Uri("https://github.com/khteh/AspNetCoreApiStarter"),
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
                {
                    {
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
                    }
                });
    c.CustomSchemaIds(i => i.FullName);
});
builder.Services.AddGrpc();
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

// WARNING: use *either* the NameUserIdProvider *or* the 
// EmailBasedUserIdProvider, but do not use both. 
// Register Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration, _isIntegrationTests).AddCore().AddOutputPorts();//.AddHealthCheck();
builder.Services.AddHealthChecks()
    .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>() { "Liveness" })
    .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string> { "Readiness" })
    .AddMySql(builder.Configuration["ConnectionStrings:Default"], "MySQL", HealthStatus.Unhealthy, new List<string> { "Services" })
    .AddDbContextCheck<AppDbContext>("AppDbContext", HealthStatus.Unhealthy, new List<string> { "Services" });
builder.Services.AddHostedService<StartupHostedService>()
    .AddSingleton<ReadinessHealthCheck>()
    .AddSingleton<LivenessHealthCheck>();
builder.Services.Configure<ForwardedHeadersOptions>(options => {
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 16)); // Load Balancer / VPC Network
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
app.UseResponseCaching();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy(new CookiePolicyOptions() { HttpOnly = HttpOnlyPolicy.Always, Secure = CookieSecurePolicy.Always });
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub", options => {
    if (!_isIntegrationTests) // Websockets is currently unmockable. https://github.com/dotnet/aspnetcore/issues/28108
        options.Transports = HttpTransportType.WebSockets;
});
//endpoints.MapGrpcService<GreeterService>("/greet");
app.MapGrpcService<AccountsService>();
app.MapGrpcService<AuthService>();
app.MapHealthChecks($"/health/live", new HealthCheckOptions()
{
    Predicate = check => check.Name == "Liveness"
});
app.MapHealthChecks($"/health/ready", new HealthCheckOptions()
{
    Predicate = check => check.Name == "Readiness"
});

app.MapRazorPages();
string pathBase = app.Configuration["PATH_BASE"];
app.UseSwagger().UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{pathBase}/swagger/v3/swagger.json", "AspNetCoreApiStarter V3");
});
app.Logger.LogInformation($"Using PathBase: {pathBase}");
app.Use(async (context, next) =>
{
    // Request method, scheme, and path
    //_logger.LogInformation($"Method: {context.Request.Method}, Scheme: {context.Request.Scheme}, PathBase: {context.Request.PathBase}, Path: {context.Request.Path}, IP: {context.Connection.RemoteIpAddress}, Host: {context.Request.Host}, ContentLength: {context.Request.ContentLength}");
    RequestLog requestLog = new RequestLog(context?.Request?.Method,
                                        context?.Request?.Scheme,
                                        context?.Request?.PathBase,
                                        context?.Request?.Path,
                                        context?.Request?.Host.ToString(),
                                        context?.Request?.ContentLength,
                                        context?.Connection?.RemoteIpAddress?.ToString(),
                                        context?.Request?.QueryString.ToString(),
                                        context?.Request?.ContentType,
                                        context?.Request?.Protocol,
                                        context?.Request?.Headers
                                        );
    app.Logger.LogInformation(requestLog.ToString());
    // Headers
    //foreach (var header in context.Request.Headers)
    //    _logger.LogInformation("Header: {KEY}: {VALUE}", header.Key, header.Value);
    // Connection: RemoteIp
    context.Request.PathBase = new PathString(pathBase); // Kubernetes ingress rule
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
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
        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false });
    }
    await next();
});

IHostApplicationLifetime lifetime = app.Lifetime;
ReadinessHealthCheck readinessHealthCheck = app.Services.GetRequiredService<ReadinessHealthCheck>();
lifetime.ApplicationStarted.Register(() => AppStarted(app.Logger, readinessHealthCheck));
lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("ApplicationStopping"));
lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("ApplicationStopped"));

app.Run();

static void AppStarted(Microsoft.Extensions.Logging.ILogger logger, ReadinessHealthCheck readinessHealthCheck)
{
    logger.LogInformation($"ApplicationStarted");
    readinessHealthCheck.StartupTaskCompleted = true;
}
public partial class Program { } // so you can reference it from tests

#if false
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            // What's the diff between env.ContentRootPath and Directory.GetCurrentDirectory()???
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == Environments.Development;
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.mysql.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            LoggerConfiguration logConfig = new LoggerConfiguration().ReadFrom.Configuration(config);
            //.MinimumLevel.Debug()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //.WriteTo.RollingFile(config["Logging:LogFile"], fileSizeLimitBytes: 10485760, retainedFileCountLimit: null)
            //.Enrich.FromLogContext();
            if (isDevelopment)
                //config.WriteTo.Console(new CompactJsonFormatter())
                logConfig.WriteTo.ColoredConsole(
                        LogEventLevel.Verbose,
                        "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
            else
                logConfig.WriteTo.Console(new ElasticsearchJsonFormatter());
            // Create the logger
            Log.Logger = logConfig.CreateLogger();
            try {
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
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
                CreateHostBuilder(args).Build().Run();
            } catch (Exception e) {
                Log.Fatal($"Exception: {e.Message}");
            } finally {
                Log.CloseAndFlush();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Program>()
                    .ConfigureAppConfiguration((hostingContext, config) => {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.mysql.json", true, true);
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.ClearProviders();
#if false
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                        logging.AddSerilog(dispose: true);
#endif
                    })
                    .UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory()))
                    // Add the Serilog ILoggerFactory to IHostBuilder
                    .UseSerilog((ctx, config) =>
                    {
                        config.ReadFrom.Configuration(ctx.Configuration);
                        if (ctx.HostingEnvironment.IsDevelopment())
                            config.WriteTo.ColoredConsole(
                                LogEventLevel.Verbose,
                                "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
                    })
                );
    }
}
#endif