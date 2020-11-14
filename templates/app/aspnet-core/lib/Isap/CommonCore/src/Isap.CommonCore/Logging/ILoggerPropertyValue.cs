using System;

namespace Isap.CommonCore.Logging
{
	public interface ILoggerPropertyValue: IDisposable
	{
		string Name { get; }
		object Value { get; }
	}
}
