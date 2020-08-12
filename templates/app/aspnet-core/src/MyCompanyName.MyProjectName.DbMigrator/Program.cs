using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace MyCompanyName.MyProjectName.DbMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("MyCompanyName.MyProjectName", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("MyCompanyName.MyProjectName", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs/logs.txt"))
                .WriteTo.Console()
                .CreateLogger();

            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var switchMappings = new Dictionary<string, string>
                {
                    { "-p", "DataProviderKey" },
                    { "-e", "EnvironmentName" },
                };

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                    {
                        config.AddCommandLine(args, switchMappings);
                    })
                .ConfigureLogging((context, logging) => logging.ClearProviders())
                .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<DbMigratorHostedService>();
                    })
                ;
        }
    }
}
