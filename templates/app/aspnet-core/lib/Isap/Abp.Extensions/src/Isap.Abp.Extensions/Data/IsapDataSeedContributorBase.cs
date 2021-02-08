using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Volo.Abp.Data;

namespace Isap.Abp.Extensions.Data
{
	public abstract class IsapDataSeedContributorBase: IDataSeedContributor, ISupportsLazyServices
	{
		public IServiceProvider ServiceProvider { get; set; }
		protected readonly object ServiceProviderLock = new object();
		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;
		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		public abstract Task SeedAsync(DataSeedContext context);

		protected TService LazyGetRequiredService<TService>()
		{
			return SupportsLazyServicesExtensions.LazyGetRequiredService<TService>(this);
		}
	}
}
