using System;
using Isap.Abp.Extensions.Domain;
using Microsoft.Extensions.Logging;

namespace Isap.Abp.Extensions.Api
{
	public abstract class DisposableDomainServiceBase: DomainServiceBase, IDisposable
	{
		private bool IsDisposed { get; set; }

		~DisposableDomainServiceBase()
		{
			if (!IsDisposed)
			{
				DomainLogger?.LogWarning($"Dispose method was not called for disposable object type {GetType().FullName}.");
			}

			Dispose();
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				InternalDispose();
				IsDisposed = true;
			}
		}

		protected abstract void InternalDispose();
	}
}
