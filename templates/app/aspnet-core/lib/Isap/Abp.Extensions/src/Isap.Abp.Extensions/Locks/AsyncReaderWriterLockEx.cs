using System;
using System.Threading;
using Nito.AsyncEx;
using Volo.Abp.Timing;

namespace Isap.Abp.Extensions.Locks
{
	public class AsyncReaderWriterLockEx
	{
		private readonly IClock _clock;
		private readonly bool _isCanBeExpired;
		private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
		private long _lastAccessTime;

		public AsyncReaderWriterLockEx(IClock clock, bool isCanBeExpired = true)
		{
			_clock = clock;
			_isCanBeExpired = isCanBeExpired;
			_lastAccessTime = (isCanBeExpired ? clock.Now : DateTime.MaxValue).Ticks;
		}

		public DateTime LastAccessTime => DateTime.FromBinary(Interlocked.Read(ref _lastAccessTime));

		public AwaitableDisposable<IDisposable> ReaderLockAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_isCanBeExpired)
				Interlocked.Exchange(ref _lastAccessTime, _clock.Now.Ticks);
			return _lock.ReaderLockAsync(cancellationToken);
		}

		public AwaitableDisposable<IDisposable> WriterLockAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_isCanBeExpired)
				Interlocked.Exchange(ref _lastAccessTime, _clock.Now.Ticks);
			return _lock.WriterLockAsync(cancellationToken);
		}
	}
}
