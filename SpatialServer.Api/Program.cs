using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SpatialServer.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpatialServer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.MigrateDatabase<Program>();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // args[0] = "./appsettings.json";
            var builtConfig = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json", false, reloadOnChange: false)
                .AddCommandLine(args)
                .Build();

            var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}][{Level}]{Message:l}{NewLine}";
            Log.Logger = new LoggerConfiguration().WriteTo.Console(
                outputTemplate: outputTemplate,
                    theme: AnsiConsoleTheme.Code
                    ).WriteTo.File(
                path: "./logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information,
              outputTemplate: outputTemplate)
          .ReadFrom.Configuration(builtConfig, sectionName: "Logging")
          .CreateLogger();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
