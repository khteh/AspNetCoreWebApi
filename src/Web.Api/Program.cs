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
global using Serilog.Events;
global using Serilog.Formatting.Elasticsearch;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Net;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
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
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
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
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
    //builder.WebHost.UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory())); Changing the host configuration using WebApplicationBuilder.Host is not supported. Use WebApplication.CreateBuilder(WebApplicationOptions) instead.
    LoggerConfiguration logConfig = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration);
    if (env.IsDevelopment() || env.IsStaging())
        //config.WriteTo.Console(new CompactJsonFormatter())
        logConfig.WriteTo.Console(LogEventLevel.Verbose, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
    else
        logConfig.WriteTo.Console(new ElasticsearchJsonFormatter());
    // Create the logger
    Log.Logger = logConfig.CreateLogger();
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

    ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
    builder.Host.UseSerilog((ctx, config) =>
                         {
                             config.ReadFrom.Configuration(ctx.Configuration);
                             if (ctx.HostingEnvironment.IsDevelopment() || ctx.HostingEnvironment.IsStaging())
                                 config.WriteTo.Console(LogEventLevel.Verbose, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}");
                             else
                                 config.WriteTo.Console(new ElasticsearchJsonFormatter());
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
    builder.Services.AddControllersWithViews();
    builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    //.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter())); Fixed in .Net Core 5
    builder.Services.AddAutoMapper(new[] { typeof(IdentityProfile), typeof(GrpcProfile), typeof(ResponseProfile) });
    builder.Services.AddMediatR(typeof(Program));
    builder.Services.AddScoped<IPipelineBehavior<RegisterUserCommand, RegisterUserResponse>, LoggingBehavior<RegisterUserCommand, RegisterUserResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<LogInCommand, LogInResponse>, LoggingBehavior<LogInCommand, LogInResponse>>();
    builder.Services.AddScoped<IPipelineBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>, LoggingBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>>();
    builder.Services.AddEndpointsApiExplorer();
    // Register the Swagger generator, defining 1 or more Swagger documents
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v7.0", new OpenApiInfo
        {
            Title = "ASP.Net Core RESTful, SignalR and GRPC service",
            Version = "v7.0",
            Description = "An ASP.NET Core 7.0 Web API, SignalR and GRPC project to quickly bootstrap new projects.  Includes Identity, JWT authentication w/ refresh tokens.",
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
    builder.Services.AddInfrastructure(builder.Configuration, _isIntegrationTests).AddCore().AddOutputPorts();
    builder.Services.AddHealthChecks()
        .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>() { "live" })
        .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string> { "ready" })
        .AddNpgSql(builder.Configuration["ConnectionStrings:Default"], "PostgreSQL")
        .AddDbContextCheck<AppDbContext>("AppDbContext", HealthStatus.Unhealthy, new List<string> { "Services" });
    builder.Services.AddHostedService<StartupBackgroundService>()
        .AddSingleton<ReadinessHealthCheck>()
        .AddSingleton<LivenessHealthCheck>();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
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
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        app.UseDeveloperExceptionPage();
    else
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
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
    app.UsePathBase(pathBase);
    app.UseRouting();
    app.UseAuthentication(); // The order in which you register the SignalR and ASP.NET Core authentication middleware matters. Always call UseAuthentication before UseSignalR so that SignalR has a user on the HttpContext.
    app.UseAuthorization();
    app.UseWebSockets();
    //if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    app.UseSwagger().UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"{pathBase}/swagger/v7.0/swagger.json", "AspNetCoreWebApi V7.0");
    });
    app.UseSerilogRequestLogging();
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
        context.Request.PathBase = new PathString(pathBase); // Kubernetes ingress rule.
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
        await next(context);
    });
    app.MapRazorPages();
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapHub<ChatHub>($"/chatHub", o => o.Transports = HttpTransportType.WebSockets);
    app.MapGrpcService<AccountsService>();
    app.MapGrpcService<AuthService>();
    app.MapHealthChecks("/health/live", new HealthCheckOptions()
    {
        Predicate = check => check.Name == "Liveness"
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions()
    {
        Predicate = check => check.Name == "Readiness"
    });

    IHostApplicationLifetime lifetime = app.Lifetime;
    ReadinessHealthCheck readinessHealthCheck = app.Services.GetRequiredService<ReadinessHealthCheck>();
    lifetime.ApplicationStarted.Register(() => AppStarted(app.Logger, readinessHealthCheck));
    lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation($"{typeof(Program).Assembly.GetName().Name} stopping..."));
    lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation($"{typeof(Program).Assembly.GetName().Name} stopped!"));

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
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