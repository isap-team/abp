using Microsoft.Extensions.DependencyInjection;

namespace Isap.Abp.Extensions.Domain
{
	public static class DomainServiceExtensions
	{
		public static TService LazyGetRequiredService<TService>(this IDomainServiceBase service)
		{
			return (TService) service.ServiceReferenceMap.GetOrAdd(typeof(TService), serviceType => service.ServiceProvider.GetRequiredService(serviceType));
		}
	}
}
