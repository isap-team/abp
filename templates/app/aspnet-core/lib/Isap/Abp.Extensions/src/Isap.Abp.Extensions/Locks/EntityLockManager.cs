using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Volo.Abp.Timing;

namespace Isap.Abp.Extensions.Locks
{
	public interface IEntityLockManager
	{
		Type EntityType { get; }
		int ReaderWriterLockMapSize { get; }

		Task RemoveObsoleteLocks(CancellationToken cancellationToken = default(CancellationToken));
	}

	public interface IEntityLockManager<in TKey>: IEntityLockManager
	{
		AsyncReaderWriterLockEx GetLock(TKey entryId, CancellationToken cancellationToken);
		AsyncReaderWriterLockEx GetLock(TKey entryId, int? acquireLockTimeout = null);

		AwaitableDisposable<IDisposable> GetReaderLockAsync(TKey entryId, CancellationToken cancellationToken);
		AwaitableDisposable<IDisposable> GetReaderLockAsync(TKey entryId, int? acquireLockTimeout = null);

		AwaitableDisposable<IDisposable> GetWriterLockAsync(TKey entryId, CancellationToken cancellationToken);
		AwaitableDisposable<IDisposable> GetWriterLockAsync(TKey entryId, int? acquireLockTimeout = null);
	}

	public static class EntityLockManager
	{
		public static TimeSpan ObsoleteInterval = TimeSpan.FromMinutes(150D);

		public static IEntityLockManager Create(Type entityType, Type keyType, ILogger logger)
		{
			Type managerType = typeof(EntityLockManager<,>).MakeGenericType(entityType, keyType);
			return (IEntityLockManager) Activator.CreateInstance(managerType, logger);
		}
	}

	public class EntityLockManager<TEntity, TKey>: IEntityLockManager<TKey>
	{
		public const int DefaultAcquireLockTimeout = 20000;

		private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
		private readonly ConcurrentDictionary<TKey, AsyncReaderWriterLockEx> _locks;

		public EntityLockManager(ILogger logger, IClock clock)
		{
			Logger = logger;
			Clock = clock;
			_locks = new ConcurrentDictionary<TKey, AsyncReaderWriterLockEx>(new Dictionary<TKey, AsyncReaderWriterLockEx>
				{
					{ default(TKey), new AsyncReaderWriterLockEx(clock, false) },
				});
		}

		public IClock Clock { get; }

		public ILogger Logger { get; }

		public Type EntityType => typeof(TEntity);

		public int ReaderWriterLockMapSize => _locks.Count;

		public AsyncReaderWriterLockEx GetLock(TKey entryId, CancellationToken cancellationToken)
		{
			using (_lock.ReaderLock(cancellationToken))
				return _locks.GetOrAdd(entryId, key => new AsyncReaderWriterLockEx(Clock));
		}

		public AsyncReaderWriterLockEx GetLock(TKey entryId, int? acquireLockTimeout = null)
		{
			var cts = new CancellationTokenSource(acquireLockTimeout ?? DefaultAcquireLockTimeout);
			return GetLock(entryId, cts.Token);
		}

		public AwaitableDisposable<IDisposable> GetReaderLockAsync(TKey entryId, CancellationToken cancellationToken)
		{
#if DEBUG
			//Logger.Debug(() => $"Try to acquire reader lock for entry id = '{entryId}' of type = '{EntityType}'.{Environment.NewLine}{new StackTrace()}");
#endif
			AsyncReaderWriterLockEx rwLock = GetLock(entryId, cancellationToken);
			return rwLock.ReaderLockAsync(cancellationToken);
		}

		public AwaitableDisposable<IDisposable> GetReaderLockAsync(TKey entryId, int? acquireLockTimeout = null)
		{
			var cts = new CancellationTokenSource(acquireLockTimeout ?? DefaultAcquireLockTimeout);
			return GetReaderLockAsync(entryId, cts.Token);
		}

		public AwaitableDisposable<IDisposable> GetWriterLockAsync(TKey entryId, CancellationToken cancellationToken)
		{
#if DEBUG
			//Logger.Debug(() => $"Try to acquire writer lock for entry id = '{entryId}' of type = '{EntityType}'.{Environment.NewLine}{new StackTrace()}");
#endif
			AsyncReaderWriterLockEx rwLock = GetLock(entryId, cancellationToken);
			return rwLock.WriterLockAsync(cancellationToken);
		}

		public AwaitableDisposable<IDisposable> GetWriterLockAsync(TKey entryId, int? acquireLockTimeout = null)
		{
			var cts = new CancellationTokenSource(acquireLockTimeout ?? DefaultAcquireLockTimeout);
			return GetWriterLockAsync(entryId, cts.Token);
		}

		public async Task RemoveObsoleteLocks(CancellationToken cancellationToken = default(CancellationToken))
		{
			int removedLocks = 0;
			using (await _lock.WriterLockAsync(cancellationToken))
			{
				DateTime now = Clock.Now;
				List<TKey> obsoleteKeys = _locks
					.Where(pair => pair.Value.LastAccessTime < now - EntityLockManager.ObsoleteInterval)
					.Select(pair => pair.Key)
					.ToList();
				foreach (TKey obsoleteKey in obsoleteKeys)
					removedLocks += _locks.TryRemove(obsoleteKey, out _) ? 1 : 0;
			}

			if (removedLocks > 0)
				Logger.LogDebug($"EntityLockManager<{EntityType.Name}>: removed {removedLocks} obsolete lock(s).");
		}
	}
}
