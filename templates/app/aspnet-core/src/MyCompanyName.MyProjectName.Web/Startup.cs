using System;
using Isap.Abp.Extensions.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Timing;

namespace MyCompanyName.MyProjectName.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MyProjectNameWebModule>(options =>
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

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeApplication();
        }
    }
}
