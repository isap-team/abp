using System.Collections.Generic;

namespace Isap.Abp.Extensions.Web
{
	public class AbpExtWebOptions
	{
		public string AbpScriptsBaseUrl { get; set; }
		public AbpExtAuthServerOptions AuthServer { get; set; }
		public List<AbpExtTenantDomainsEntry> TenantDomainsMap { get; set; }
	}

	public class AbpExtAuthServerOptions
	{
		public string Authority { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string ApiName { get; set; }
	}

	public class AbpExtTenantDomainsEntry
	{
		public string TenantIdOrName { get; set; }
		public List<string> Domains { get; set; }
	}
}
