using Isap.Abp.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Isap.Abp.Extensions.Web
{
	[DependsOn(
		typeof(AbpAspNetCoreMvcUiModule),
		typeof(AbpAspNetCoreMultiTenancyModule))]
	public class IsapAbpExtensionsWebModule: AbpModule
	{
		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
				{
					options.AddAssemblyResource(typeof(AbpExtensionsResource), typeof(IsapAbpExtensionsWebModule).Assembly);
				});

			PreConfigure<IMvcBuilder>(mvcBuilder =>
				{
					// mvcBuilder.AddNewtonsoftJson();
					mvcBuilder.AddApplicationPartIfNotExists(typeof(IsapAbpExtensionsWebModule).Assembly);
				});
		}

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			base.ConfigureServices(context);

			Configure<AbpVirtualFileSystemOptions>(options =>
				{
					options.FileSets.AddEmbedded<IsapAbpExtensionsWebModule>();
				});

			Configure<AbpNavigationOptions>(options =>
				{
					options.MenuContributors.Add(new IsapAbpExtensionsWebMenuContributor());
				});
		}
	}
}
