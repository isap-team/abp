using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Isap.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MyCompanyName.MyProjectName
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("App_Data/Logs/logs.txt"))
#if DEBUG
                .WriteTo.Logger(lc => lc
                    .WriteTo
                    .Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
                    .Filter.With<HostingLifetimeFilter>()
                )
#endif
                .CreateLogger();

            try
            {
                Log.Information("Starting MyCompanyName.MyProjectName.HttpApi.Host.");
                IHostBuilder hostBuilder = CreateHostBuilder(args);
                await hostBuilder.BuildAndRunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            string contentRootDir = Directory.GetCurrentDirectory();

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootDir)
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                        webBuilder
                            .UseContentRoot(contentRootDir)
                            .UseUrls("https://*:44305")
                            .UseConfiguration(config)
                            .UseStartup<Startup>();
                })
                .UseAutofac(containerBuilder => containerBuilder.RegisterModule<IsapAutofacModule>())
                .UseSerilog();
    }
    }
}
