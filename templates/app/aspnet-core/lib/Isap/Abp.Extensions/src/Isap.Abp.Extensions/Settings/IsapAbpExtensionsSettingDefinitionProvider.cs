using Volo.Abp.Settings;

namespace Isap.Abp.Extensions.Settings
{
	public class IsapAbpExtensionsSettingDefinitionProvider: SettingDefinitionProvider
	{
		public override void Define(ISettingDefinitionContext context)
		{
			context.Add(new SettingDefinition(IsapAbpExtensionsSettings.UnregisteredUserId));
		}
	}
}
