using System;
using System.Collections.Concurrent;

namespace Isap.Abp.Extensions.Domain
{
	public interface ISupportsLazyServices
	{
		IServiceProvider ServiceProvider { get; }
		object ServiceProviderLock { get; }
		ConcurrentDictionary<Type, object> ServiceReferenceMap { get; }
	}
}
