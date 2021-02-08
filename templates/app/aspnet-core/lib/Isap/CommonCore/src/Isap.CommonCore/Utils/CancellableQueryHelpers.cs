using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Isap.CommonCore.Utils
{
	public static class CancellableQueryHelpers
	{
		public static async Task<T> Query<T>(Func<CancellationToken, Task<T>> query, TimeSpan queryTimeout, int tryCount = 3,
			CancellationToken cancellationToken = default)
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (tryCount-- <= 0)
					throw new InvalidOperationException("Too many cancelled queries.");

				try
				{
					var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(queryTimeout).Token);
					return await query(cts.Token);
				}
				catch (AggregateException exception)
				{
					if (cancellationToken.IsCancellationRequested)
						throw;

					if (exception.InnerExceptions.OfType<OperationCanceledException>().Any())
						continue;

					throw;
				}
				catch (OperationCanceledException)
				{
					if (cancellationToken.IsCancellationRequested)
						throw;
				}
			}
		}

		public static async Task<T> Query<T>(Func<CancellationToken, Task<T>> query, long queryTimeoutMilliseconds = 15000, int tryCount = 3,
			CancellationToken cancellationToken = default)
		{
			return await Query(query, TimeSpan.FromMilliseconds(queryTimeoutMilliseconds), tryCount, cancellationToken);
		}
	}
}
