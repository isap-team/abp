using System.Threading;

namespace Isap.Abp.Extensions.Globalization
{
	/// <summary>
	///     SynchronizationContext that passes around current thread's culture
	/// </summary>
	public class CultureAwareSynchronizationContext: SynchronizationContext
	{
		private readonly ThreadCultureHolder _cultureHolder;
		private readonly SynchronizationContext _synchronizationImplementation;

		/// <summary>
		///     Creates default SynchronizationContext, using current(previous) SynchronizationContext
		///     and captures culture information from currently running thread
		/// </summary>
		public CultureAwareSynchronizationContext()
			: this(Current)
		{
		}

		/// <summary>
		///     Uses passed SynchronizationContext (or null, in that case creates new empty SynchronizationContext)
		///     and captures culture information from currently running thread
		/// </summary>
		/// <param name="previous"></param>
		public CultureAwareSynchronizationContext(SynchronizationContext previous)
			: this(new ThreadCultureHolder(), previous)
		{
		}

		internal CultureAwareSynchronizationContext(ThreadCultureHolder currentCultureHolder, SynchronizationContext currentSynchronizationContext)
		{
			_cultureHolder = currentCultureHolder;
			_synchronizationImplementation = currentSynchronizationContext ?? new SynchronizationContext();
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			_cultureHolder.ApplyCulture();
			_synchronizationImplementation.Send(d, state);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			_synchronizationImplementation.Post(passedState =>
				{
					SetSynchronizationContext(this);
					_cultureHolder.ApplyCulture();
					d.Invoke(passedState);
				}, state);
		}

		public override SynchronizationContext CreateCopy()
		{
			return new CultureAwareSynchronizationContext(_cultureHolder, _synchronizationImplementation.CreateCopy());
		}

		public override string ToString()
		{
			return string.Format("CultureAwareSynchronizationContext: {0}", _cultureHolder);
		}
	}
}
