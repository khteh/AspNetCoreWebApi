using System.Security.Cryptography;
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
using Microsoft.Extensions.Hosting;
using System.Net;
using Serilog.Settings.Configuration;
using System.Threading;

namespace Web.Api
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
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>()
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