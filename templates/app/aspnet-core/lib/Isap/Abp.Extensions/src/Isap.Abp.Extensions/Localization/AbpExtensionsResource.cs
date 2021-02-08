using Volo.Abp.Localization;

namespace Isap.Abp.Extensions.Localization
{
	[LocalizationResourceName(ResourceName)]
	public class AbpExtensionsResource: ILocalizationResource
	{
		/// <summary>
		/// Каталог с файлами локализаций
		/// </summary>
		public const string ResourceName = "AbpExtensions";

	}
}
