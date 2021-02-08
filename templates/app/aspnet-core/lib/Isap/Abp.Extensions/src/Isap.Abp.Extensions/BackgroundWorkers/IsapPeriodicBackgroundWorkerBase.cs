using System;
using System.Collections.Concurrent;
using Isap.Abp.Extensions.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Isap.Abp.Extensions.BackgroundWorkers
{
	public abstract class IsapPeriodicBackgroundWorkerBase: AsyncPeriodicBackgroundWorkerBase, ISupportsLazyServices
	{
		protected IsapPeriodicBackgroundWorkerBase(AbpTimer timer, IServiceScopeFactory serviceScopeFactory)
			: base(timer, serviceScopeFactory)
		{
		}

		IServiceProvider ISupportsLazyServices.ServiceProvider => ServiceProvider;
		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;
		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
		}
	}
}
