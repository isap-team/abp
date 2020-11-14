using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Isap.CommonCore;
using Isap.CommonCore.DependencyInjection;
using Isap.Converters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class DomainServiceBase: DomainService, IDomainServiceBase, ICommonInitialize
	{
		private static long _lastObjectInstanceId;

		private static readonly IValueConverter _defaultValueConverter = ValueConverterProviders.Default.GetConverter();

		protected DomainServiceBase()
		{
			ObjectInstanceId = Interlocked.Increment(ref _lastObjectInstanceId);
			Converter = _defaultValueConverter;
			//PermissionChecker = NullPermissionChecker.Instance;
		}

		ConcurrentDictionary<Type, object> IDomainServiceBase.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		public long ObjectInstanceId { get; }

		public IUnitOfWorkManager UnitOfWorkManager => LazyGetRequiredService<IUnitOfWorkManager>();

		[PropertyInject]
		public IValueConverter Converter { get; set; }

		//public IPermissionChecker PermissionChecker { get; set; }

		public IUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

		public IOptions<AbpExceptionLocalizationOptions> LocalizationOptions => LazyGetRequiredService<IOptions<AbpExceptionLocalizationOptions>>();

		public IStringLocalizerFactory StringLocalizerFactory => LazyGetRequiredService<IStringLocalizerFactory>();

		protected ILogger DomainLogger { get; private set; }

		public virtual void Initialize()
		{
			DomainLogger = Logger;
		}

		protected TService LazyGetRequiredService<TService>()
		{
			return DomainServiceExtensions.LazyGetRequiredService<TService>(this);
		}

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
	}
}
