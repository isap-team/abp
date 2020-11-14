using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;
using Volo.Abp.Localization.ExceptionHandling;

namespace Isap.Abp.Extensions.Services
{
	public abstract class AppServiceBase: ApplicationService
	{
		private readonly ConcurrentDictionary<Type, object> _serviceReferenceMap = new ConcurrentDictionary<Type, object>();

		public IOptions<AbpExceptionLocalizationOptions> LocalizationOptions => LazyGetRequiredService<IOptions<AbpExceptionLocalizationOptions>>();

		protected TService LazyGetRequiredService<TService>()
		{
			return (TService) _serviceReferenceMap.GetOrAdd(typeof(TService), serviceType => ServiceProvider.GetRequiredService(serviceType));
		}

		/*
		protected virtual string L(string name, params object[] args)
		{
			if (name.IsNullOrWhiteSpace() || !name.Contains(":"))
				return name;

			string[] nameItems = name.Split(new[] { ':' }, 2);
			string @namespace = nameItems[0];
			string key = nameItems[1];

			Type localizationResourceType = LocalizationOptions.Value.ErrorCodeNamespaceMappings.GetOrDefault(@namespace);
			if (localizationResourceType == null)
				return name;

			var stringLocalizer = StringLocalizerFactory.Create(localizationResourceType);
			var localizedString = stringLocalizer[key, args];
			if (localizedString.ResourceNotFound)
			{
				return name;
			}

			return localizedString.Value;
		}
		*/

		protected virtual List<DataFilterValue> ToDataFilterValues(ICollection<DataFilterValueDto> filterValues)
		{
			return filterValues?.Select(e => ObjectMapper.Map<DataFilterValueDto, DataFilterValue>(e)).ToList();
		}

		protected virtual ICollection<SortOption> ToSortOptions(ICollection<SortOptionDto> sortOptions)
		{
			return sortOptions?.Select(e => ObjectMapper.Map<SortOptionDto, SortOption>(e)).ToList();
		}
	}

	public abstract class AppServiceBase<TEntityDto, TIntf>: AppServiceBase
	{
		protected virtual TEntityDto ToDto(TIntf entry)
		{
			return ObjectMapper.Map<TIntf, TEntityDto>(entry);
		}
	}
}
