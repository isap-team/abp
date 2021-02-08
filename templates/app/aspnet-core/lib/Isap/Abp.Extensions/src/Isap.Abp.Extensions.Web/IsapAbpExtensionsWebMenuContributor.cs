using System.Threading.Tasks;
using Isap.Abp.Extensions.Identity;
using Isap.Abp.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Isap.Abp.Extensions.Web
{
	public class IsapAbpExtensionsWebMenuContributor: IMenuContributor
	{
		public async Task ConfigureMenuAsync(MenuConfigurationContext context)
		{
			switch (context.Menu.Name)
			{
				case StandardMenus.Main:
					await ConfigureMainMenuAsync(context);
					break;
				case StandardMenus.User:
					await ConfigureUserMenuAsync(context);
					break;
			}
		}

		protected virtual async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
		{
			IStringLocalizer l = context.GetLocalizer<AbpExtensionsResource>();
			ICurrentUser currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

			ApplicationMenuItem administration = context.Menu.GetAdministration();

			if (currentUser.IsAuthenticated)
			{
				if (currentUser.IsInRole(IsapIdentityConsts.StandardRoles.Administrator))
				{
					var diagnosticsMenuItem = new ApplicationMenuItem("Configuration", l["Menu:Configuration"], icon: "fa fa-tools", order: 1500);
					administration.Items.Add(diagnosticsMenuItem);
					diagnosticsMenuItem.Items.Add(
						new ApplicationMenuItem("AbpExtensions.Configuration.SecureString", l["Menu:Configuration.SecureString"],
							"~/configuration/securestring", icon: "fa fa-eye-slash", order: 1000)
					);
				}
			}

			await Task.Yield();
		}

		protected virtual async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
		{
			await Task.Yield();
		}
	}
}
