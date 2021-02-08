using Isap.Abp.FileStorage.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Isap.Abp.FileStorage
{
	[DependsOn(
		typeof(AbpValidationModule)
	)]
	public class FileStorageDomainSharedModule: AbpModule
	{
		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
				{
					options.FileSets.AddEmbedded<FileStorageDomainSharedModule>();
				});

			Configure<AbpLocalizationOptions>(options =>
				{
					options.Resources
						.Add<FileStorageResource>("en")
						.AddBaseTypes(typeof(AbpValidationResource))
						.AddVirtualJson("/Localization/FileStorage");
				});

			Configure<AbpExceptionLocalizationOptions>(options =>
				{
					options.MapCodeNamespace("FileStorage", typeof(FileStorageResource));
				});
		}
	}
}
