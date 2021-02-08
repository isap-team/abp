using Isap.Abp.BackgroundJobs.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Isap.Abp.BackgroundJobs
{
	[DependsOn(
		typeof(IsapAbpBackgroundJobsDomainSharedModule),
		typeof(AbpBackgroundJobsModule),
		typeof(AbpAutoMapperModule)
	)]
	public class IsapAbpBackgroundJobsDomainModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.AddAutoMapperObjectMapper<IsapAbpBackgroundJobsDomainModule>();
			Configure<AbpAutoMapperOptions>(options =>
				{
					options.AddProfile<BackgroundJobsDomainAutoMapperProfile>(validate: true);
				});
		}
	}
}
