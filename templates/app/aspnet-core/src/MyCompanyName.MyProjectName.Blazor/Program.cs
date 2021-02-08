using System.Threading.Tasks;
using Autofac;
using Isap.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MyCompanyName.MyProjectName.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var application = builder.AddApplication<MyProjectNameBlazorModule>(options =>
            {
                options
                    .UseAutofac(containerBuilder => containerBuilder.RegisterModule<IsapAutofacModule>())
                    ;
            });

            var host = builder.Build();

            await application.InitializeAsync(host.Services);

            await host.RunAsync();
        }
    }
}
