using Isap.Abp.FileStorage.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(IsapAbpFileStorageApplicationContractsModule),
		typeof(AbpAspNetCoreMvcModule))]
	public class FileStorageHttpApiModule: AbpModule
	{
		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			PreConfigure<IMvcBuilder>(mvcBuilder =>
				{
					mvcBuilder.AddApplicationPartIfNotExists(typeof(FileStorageHttpApiModule).Assembly);
				});
		}

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			Configure<AbpLocalizationOptions>(options =>
				{
					options.Resources
						.Get<FileStorageResource>()
						.AddBaseTypes(typeof(AbpUiResource));
				});

			ConfigureConventionalControllers();
		}

		private void ConfigureConventionalControllers()
		{
			Configure<AbpAspNetCoreMvcOptions>(options =>
				{
					options.ConventionalControllers.Create(typeof(FileStorageHttpApiModule).Assembly, opts =>
						{
							opts.RootPath = "fileStorage";
						});
				});
		}
	}
}
