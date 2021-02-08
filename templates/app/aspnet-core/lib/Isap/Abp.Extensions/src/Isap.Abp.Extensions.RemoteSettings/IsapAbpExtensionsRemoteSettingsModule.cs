using Volo.Abp.Modularity;
using Volo.Abp.Settings;

namespace Isap.Abp.Extensions
{
	[DependsOn(
		typeof(AbpSettingsModule))]
    public class IsapAbpExtensionsRemoteSettingsModule: AbpModule
    {
    }
}
