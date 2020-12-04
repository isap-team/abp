using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Web
{
	public class DomainNameTenantResolveContributor: HttpTenantResolveContributorBase
	{
		private static readonly Dictionary<string, Guid> _domainNameMap = new Dictionary<string, Guid>
			{
				{ "localhost:7000", new Guid("841a2213-885a-4c91-bc27-ea38f627414b") },
			};

		public override string Name => "DomainName";

		protected override async Task<string> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
		{
			await Task.Yield();
			string host = httpContext.Request.Host.ToString();
			if (_domainNameMap.TryGetValue(host, out Guid tenantId))
			{
				httpContext.Response.Cookies.Append(context.GetAbpAspNetCoreMultiTenancyOptions().TenantKey, tenantId.ToString());
				return tenantId.ToString();
			}

			string referer = httpContext.Request.Headers["Referer"].FirstOrDefault();
			if (!string.IsNullOrEmpty(referer))
			{
				var uri = new Uri(referer);
				host = uri.Authority;
				if (_domainNameMap.TryGetValue(host, out tenantId))
				{
					httpContext.Response.Cookies.Append(context.GetAbpAspNetCoreMultiTenancyOptions().TenantKey, tenantId.ToString());
					return tenantId.ToString();
				}
			}

			return null;
		}
	}
}
