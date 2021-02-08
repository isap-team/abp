using System;
using System.Collections.Concurrent;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class IsapControllerBase<TLocalizationResource>: AbpController, ISupportsLazyServices
		where TLocalizationResource: ILocalizationResource
	{
		protected IsapControllerBase()
		{
			LocalizationResource = typeof(TLocalizationResource);
		}

		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;

		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
		}
	}

	public abstract class IsapControllerBase: IsapControllerBase<AbpExtensionsResource>
	{
	}
}
