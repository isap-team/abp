using System;
using System.Diagnostics;

namespace Isap.CommonCore
{
	public abstract class DisposableBase: IDisposable
	{
		protected DisposableBase()
		{
#if DEBUG
			CreationStackTrace = new StackTrace(1).ToString();
#endif
		}

		public bool Disposed { get; private set; }

#if DEBUG
		protected string CreationStackTrace { get; }
#endif

		#region IDisposable Implementations

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~DisposableBase()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Disposed) return;
			if (disposing)
				InternalDispose();
			else
				InternalFinalize();
			Disposed = true;
		}

		protected abstract void InternalDispose();

		protected virtual void InternalFinalize()
		{
#if DEBUG
			Debug.Assert(false, string.Format("Object of type '{1}' should be disposed.{0}Created{2}", Environment.NewLine, GetType(), CreationStackTrace));
#endif
		}
	}

	public abstract class DisposableBase<T>: DisposableBase, IDisposable<T>
	{
		T IDisposable<T>.Object => GetWrappedObject();

		protected virtual T GetWrappedObject()
		{
			return (T) (object) this;
		}
	}
}
