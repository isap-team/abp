using Isap.Abp.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class IsapControllerBase<TLocalizationResource>: AbpController
		where TLocalizationResource: ILocalizationResource
	{
		protected IsapControllerBase()
		{
			LocalizationResource = typeof(TLocalizationResource);
		}
	}

	public abstract class IsapControllerBase: IsapControllerBase<AbpExtensionsResource>
	{
	}
}
