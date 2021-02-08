using System;
using Isap.Abp.Extensions.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Timing;

namespace MyCompanyName.MyProjectName
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MyProjectNameHttpApiHostModule>(options =>
                {
                    options.Services.Configure<AbpExtDbOptions>(dbOptions =>
                        {
                            dbOptions.IsMigrationMode = false;
                            dbOptions.DataProviderKey = "PostgreSql";
                        });

                    options.Services.Configure<AbpClockOptions>(clockOptions =>
                        {
                            clockOptions.Kind = DateTimeKind.Utc;
                        });
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}
