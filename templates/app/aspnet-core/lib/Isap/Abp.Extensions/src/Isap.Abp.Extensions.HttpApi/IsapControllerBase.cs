using System;
using System.Collections.Concurrent;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class IsapControllerBase<TLocalizationResource>: AbpController, IDomainServiceBase
		where TLocalizationResource: ILocalizationResource
	{
		protected IsapControllerBase()
		{
			LocalizationResource = typeof(TLocalizationResource);
		}

		ConcurrentDictionary<Type, object> IDomainServiceBase.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();
	}

	public abstract class IsapControllerBase: IsapControllerBase<AbpExtensionsResource>
	{
	}
}
