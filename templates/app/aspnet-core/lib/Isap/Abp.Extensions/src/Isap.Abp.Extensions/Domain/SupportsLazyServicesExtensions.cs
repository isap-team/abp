using Microsoft.Extensions.DependencyInjection;

namespace Isap.Abp.Extensions.Domain
{
	public static class SupportsLazyServicesExtensions
	{
		public static TService LazyGetRequiredService<TService>(this ISupportsLazyServices service)
		{
			lock (service.ServiceProviderLock)
				return (TService) service.ServiceReferenceMap.GetOrAdd(typeof(TService), serviceType => service.ServiceProvider.GetRequiredService(serviceType));
		}
	}
}
