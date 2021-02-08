using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.MultiTenancy;
using Isap.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Web
{
	public class DomainNameTenantResolveContributor: HttpTenantResolveContributorBase
	{
		private readonly AsyncReaderWriterLock _rwLock = new AsyncReaderWriterLock();

		private readonly Dictionary<string, Guid> _domainNameMap = new Dictionary<string, Guid>
			{
				{ "localhost:7000", IsapMultiTenancyConsts.DefaultTenant.Id },
			};

		private Lazy<ILogger> _lazyLogger;
		private bool _isInitialized;

		public DomainNameTenantResolveContributor()
		{
			LoggerFactory = null;
			_lazyLogger = new Lazy<ILogger>(() => NullLogger.Instance, true);
		}

		protected ILoggerFactory LoggerFactory { get; private set; }
		protected ILogger Logger => _lazyLogger.Value;

		protected IValueConverter Converter { get; private set; }
		protected ITenantCache TenantCache { get; private set; }

		public override string Name => "DomainName";

		protected async Task EnsureInitialized(IServiceProvider services)
		{
			using (await _rwLock.ReaderLockAsync())
			{
				if (_isInitialized)
					return;
			}

			using (await _rwLock.WriterLockAsync())
			{
				if (_isInitialized)
					return;

				await Initialize(services);
				_isInitialized = true;
			}
		}

		protected virtual async Task Initialize(IServiceProvider services)
		{
			LoggerFactory = services.GetService<ILoggerFactory>();
			_lazyLogger = new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);

			Converter = services.GetRequiredService<IValueConverter>();
			TenantCache = services.GetRequiredService<ITenantCache>();

			var options = services.GetService<IOptions<AbpExtWebOptions>>()?.Value;
			if (options?.TenantDomainsMap != null)
			{
				Logger.Log(LogLevel.Information, "Start preparing host to tenant map.");

				foreach (AbpExtTenantDomainsEntry entry in options.TenantDomainsMap)
				{
					ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(entry.TenantIdOrName);

					ITenantBase tenant = null;
					if (attempt.IsSuccess)
						tenant = await TenantCache.GetOrNullAsync(attempt.Result);
					tenant ??= await TenantCache.GetOrNullAsync(entry.TenantIdOrName);
					if (tenant == null)
					{
						Logger.Log(LogLevel.Warning, $"Tenant with Id or Name = '{entry.TenantIdOrName}' is not registered in database.");
						continue;
					}
					entry.Domains.ForEach(host =>
						{
							_domainNameMap[host] = tenant.Id;
							Logger.Log(LogLevel.Information, $"Tenant with Id = '{tenant.Id}' is successfully registered for host '{host}'.");
						});
				}
			}
		}

		protected override async Task<string> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
		{
			await EnsureInitialized(context.ServiceProvider);

			string host = httpContext.Request.Host.ToString();
			Logger.Log(LogLevel.Debug, $"Try to resolve tenant for host '{host}'.");

			if (_domainNameMap.TryGetValue(host, out Guid tenantId))
			{
				Logger.Log(LogLevel.Debug, $"Tenant with Id = '{tenantId}' is successfully resolved for host '{host}'.");
				httpContext.Response.Cookies.Append(context.GetAbpAspNetCoreMultiTenancyOptions().TenantKey, tenantId.ToString());
				return tenantId.ToString();
			}

			string referer = httpContext.Request.Headers["Referer"].FirstOrDefault();
			Logger.Log(LogLevel.Debug, $"Try to resolve tenant for referer host '{host}'.");

			if (!string.IsNullOrEmpty(referer))
			{
				var uri = new Uri(referer);
				host = uri.Authority;
				if (_domainNameMap.TryGetValue(host, out tenantId))
				{
					Logger.Log(LogLevel.Debug, $"Tenant with Id = '{tenantId}' is successfully resolved for referer host '{host}'.");
					httpContext.Response.Cookies.Append(context.GetAbpAspNetCoreMultiTenancyOptions().TenantKey, tenantId.ToString());
					return tenantId.ToString();
				}
			}

			return null;
		}
	}
}
