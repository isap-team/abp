using System;
using System.Threading;
using Volo.Abp.Timing;

namespace Isap.Abp.Extensions.Data
{
	public static class RevisionHelpers
	{
		private static long _lastRevision;
		private static readonly ReaderWriterLock _rwLock = new ReaderWriterLock();

		public static long GetNextRevision(IClock clock)
		{
			_rwLock.AcquireWriterLock(TimeSpan.Zero);
			try
			{
				_lastRevision = Math.Max(clock.Now.Ticks, _lastRevision + 1);
				return _lastRevision;
			}
			finally
			{
				_rwLock.ReleaseWriterLock();
			}
		}
	}
}
