using System;
using System.Diagnostics;
using System.Threading;
using Isap.CommonCore;
using Isap.Converters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Localization;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class DomainServiceBase: DomainService, IDomainServiceBase, ICommonInitialize
	{
		private static long _lastObjectInstanceId;

		private IStringLocalizer _localizer;
		private Type _localizationResource = typeof(DefaultResource);

		protected DomainServiceBase()
		{
			ObjectInstanceId = Interlocked.Increment(ref _lastObjectInstanceId);
			//PermissionChecker = NullPermissionChecker.Instance;
		}

		public long ObjectInstanceId { get; }

		public IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

		protected IValueConverter Converter => LazyServiceProvider.LazyGetService(ValueConverterProviders.Default.GetConverter());

		//public IPermissionChecker PermissionChecker { get; set; }

		public IUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

		public IStringLocalizerFactory StringLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IStringLocalizerFactory>();

		protected ILogger DomainLogger { get; private set; }

		protected IStringLocalizer L => _localizer ?? (_localizer = CreateLocalizer());

		protected Type LocalizationResource
		{
			get => _localizationResource;
			set
			{
				_localizationResource = value;
				_localizer = null;
			}
		}

		public virtual void Initialize()
		{
			Debug.Assert(ServiceProvider != null);
			DomainLogger = Logger;
		}

		protected virtual IStringLocalizer CreateLocalizer()
		{
			if (LocalizationResource != null)
			{
				return StringLocalizerFactory.Create(LocalizationResource);
			}

			var localizer = StringLocalizerFactory.CreateDefaultOrNull();
			if (localizer == null)
			{
				throw new AbpException(
					$"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(AbpLocalizationOptions)}.{nameof(AbpLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
			}

			return localizer;
		}
	}
}
