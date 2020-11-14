using System;
using System.Collections.Concurrent;

namespace Isap.Abp.Extensions.Domain
{
	public interface IDomainServiceBase
	{
		IServiceProvider ServiceProvider { get; }
		ConcurrentDictionary<Type, object> ServiceReferenceMap { get; }
	}
}
