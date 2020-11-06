using System.Globalization;
using System.Threading;

namespace Isap.Abp.Extensions.Globalization
{
	/// <summary>
	///     Class that captures current thread's culture, and is able to reapply it to different one
	/// </summary>
	internal sealed class ThreadCultureHolder
	{
		private readonly CultureInfo _threadCulture;
		private readonly CultureInfo _threadUiCulture;

		/// <summary>
		///     Captures culture from currently running thread
		/// </summary>
		public ThreadCultureHolder()
		{
			_threadCulture = Thread.CurrentThread.CurrentCulture;
			_threadUiCulture = Thread.CurrentThread.CurrentUICulture;
		}

		/// <summary>
		///     Applies stored thread culture to current thread
		/// </summary>
		public void ApplyCulture()
		{
			Thread.CurrentThread.CurrentCulture = _threadCulture;
			Thread.CurrentThread.CurrentUICulture = _threadUiCulture;
		}

		public override string ToString()
		{
			return string.Format("{0}, UI: {1}", _threadCulture.Name, _threadUiCulture.Name);
		}
	}
}
