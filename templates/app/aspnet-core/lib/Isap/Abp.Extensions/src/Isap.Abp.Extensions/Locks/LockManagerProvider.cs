using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Timing;

namespace Isap.Abp.Extensions.Locks
{
	public interface ILockManagerProvider
	{
		IEntityLockManager Get(Type entityType, Type keyType);
		IEntityLockManager<TKey> Get<TEntity, TKey>() where TEntity: class;
		Task RemoveObsoleteLocks(CancellationToken cancellationToken = default(CancellationToken));
	}

	public class LockManagerProvider: ILockManagerProvider, ISingletonDependency
	{
		private readonly ConcurrentDictionary<Type, IEntityLockManager> _managers = new ConcurrentDictionary<Type, IEntityLockManager>();

		public ILogger<LockManagerProvider> Logger { get; set; }
		public IClock Clock { get; set; }

		public IEntityLockManager Get(Type entityType, Type keyType)
		{
			Debug.Assert(typeof(IEntity<>).MakeGenericType(keyType).IsAssignableFrom(entityType));
			return _managers.GetOrAdd(entityType, t => EntityLockManager.Create(entityType, keyType, Logger));
		}

		public IEntityLockManager<TKey> Get<TEntity, TKey>()
			where TEntity: class
		{
			Type entityType = typeof(TEntity);
			return (IEntityLockManager<TKey>) _managers.GetOrAdd(entityType, t => new EntityLockManager<TEntity, TKey>(Logger, Clock));
		}

		public async Task RemoveObsoleteLocks(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<IEntityLockManager> lockManagers = _managers.Values.ToList();
			await Task.WhenAll(lockManagers.Select(lockManager => lockManager.RemoveObsoleteLocks(cancellationToken)).ToArray());
		}
	}
}
