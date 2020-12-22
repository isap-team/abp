using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Timing;

namespace MyCompanyName.MyProjectName.PublicGateway
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplication<MyProjectNamePublicGatewayModule>(options =>
				{
					options.Services.Configure<AbpClockOptions>(clockOptions =>
						{
							clockOptions.Kind = DateTimeKind.Utc;
						});
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.InitializeApplication();
		}
	}
}
