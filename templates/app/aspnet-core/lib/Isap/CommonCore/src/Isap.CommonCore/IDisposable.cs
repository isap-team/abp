using System;

namespace Isap.CommonCore
{
	public interface IDisposable<T>: IDisposable
	{
		T Object { get; }
	}
}
