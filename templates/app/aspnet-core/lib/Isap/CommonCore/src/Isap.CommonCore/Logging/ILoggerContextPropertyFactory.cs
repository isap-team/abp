using Castle.Core.Logging;

namespace Isap.CommonCore.Logging
{
	public interface ILoggerContextPropertyFactory
	{
		ILoggerPropertyValue Create(ILogger logger, string name, object value);
	}
}
