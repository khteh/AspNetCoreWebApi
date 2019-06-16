using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace Web.Api
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            // What's the diff between env.ContentRootPath and Directory.GetCurrentDirectory()???
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == EnvironmentName.Development;
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddJsonFile($"appsettings.mysql.json", true, true)
                .AddEnvironmentVariables().Build();
            LoggerConfiguration logConfig = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    //.WriteTo.RollingFile(config["Logging:LogFile"], fileSizeLimitBytes: 10485760, retainedFileCountLimit: null)
                    .Enrich.FromLogContext();
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
                CreateWebHostBuilder(args).Build().Run();
            } catch (Exception e) {
                Log.Fatal($"Exception: {e.Message}");
            } finally {
                Log.CloseAndFlush();
            }
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) => {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.mysql.json", true, true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            }).ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
                logging.AddSerilog(dispose: true);
            })
            .UseSerilog() // Add the Serilog ILoggerFactory to IHostBuilder
            .UseStartup<Startup>()
            .ConfigureKestrel((context, options) =>
            {
                options.Listen(IPAddress.Any, 5000, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    //listenOptions.UseHttps("testCert.pfx", "testPassword");
                });
            });
    }
}