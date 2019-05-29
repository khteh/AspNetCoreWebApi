//using NLog;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;

namespace Web.Api.Infrastructure.Logging
{
    #if false
    public class Logger : Core.Interfaces.Services.ILogger
    {
        public Logger(IHostingEnvironment env)
        {
            LoggerConfiguration config = new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                       .Enrich.FromLogContext();
            if (env.IsDevelopment())
                //config.WriteTo.Console(new CompactJsonFormatter())
                config.WriteTo.Console();
            else
                config.WriteTo.Console(new ElasticsearchJsonFormatter());
            Log.Logger = config.CreateLogger();
        }
        public void LogDebug(string message) => Log.Debug(message);
        public void LogError(string message) =>  Log.Error(message);
        public void LogInfo(string message) => Log.Information(message);
        public void LogWarn(string message) => Log.Warning(message);
    }
    #endif
}