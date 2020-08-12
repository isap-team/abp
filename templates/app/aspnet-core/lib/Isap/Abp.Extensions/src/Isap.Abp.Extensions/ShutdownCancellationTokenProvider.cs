using System.Threading;

namespace Isap.Abp.Extensions
{
	public interface IShutdownCancellationTokenProvider
	{
		CancellationToken CancellationToken { get; }
	}

	public interface IShutdownCancellationTokenManager
	{
		void EnterShutdownMode();
	}

	public class ShutdownCancellationTokenProvider: IShutdownCancellationTokenProvider, IShutdownCancellationTokenManager
	{
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		#region Implementation of IShutdownCancellationTokenProvider

		public CancellationToken CancellationToken => _cts.Token;

		#endregion

		#region Implementation of IShutdownCancellationTokenManager

		public void EnterShutdownMode()
		{
			if (!_cts.IsCancellationRequested)
				_cts.Cancel();
		}

		#endregion
	}
}
