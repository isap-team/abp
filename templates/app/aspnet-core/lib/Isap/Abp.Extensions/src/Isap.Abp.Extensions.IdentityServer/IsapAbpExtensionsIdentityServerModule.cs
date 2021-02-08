using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.IdentityServer
{
	[DependsOn(
		typeof(AbpIdentityServerDomainModule)
	)]
	public class IsapAbpExtensionsIdentityServerModule: AbpModule
	{
		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			context.Services.PreConfigure<IIdentityServerBuilder>(
				builder =>
					{
						builder.AddExtensionGrantValidator<ImpersonatedClientCredentialsGrantValidator>();
					}
			);
		}
	}
}
