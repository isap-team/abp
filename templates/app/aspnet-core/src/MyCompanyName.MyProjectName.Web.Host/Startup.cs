using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Timing;

namespace MyCompanyName.MyProjectName.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MyProjectNameWebModule>(options =>
                {
                    options.Services.Configure<AbpClockOptions>(clockOptions =>
                        {
                            clockOptions.Kind = DateTimeKind.Utc;
                        });
                });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}
