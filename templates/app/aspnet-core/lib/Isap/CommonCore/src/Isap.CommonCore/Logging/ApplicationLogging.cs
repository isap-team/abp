using System;
using Castle.Core.Logging;
using Isap.Converters;

namespace Isap.CommonCore.Logging
{
	public static class ApplicationLogging
	{
		private static readonly AsyncLocalStackContainer<ILogger> _convertersStackContainer = new AsyncLocalStackContainer<ILogger>(GetDefault);

		public static ILoggerFactory LoggerFactory { get; set; }

		public static ILogger SetLogicalProperty(this ILogger logger, string name, object value)
		{
			if (logger is IExtendedLogger extendedLogger)
				extendedLogger.ThreadProperties[name] = value;
			return logger;
		}

		public static IDisposable UseInScope(ILogger logger)
		{
			return _convertersStackContainer.Use(baseLogger => logger);
		}

		public static ILogger GetScopedLogger()
		{
			return _convertersStackContainer.Current;
		}

		private static ILogger GetDefault() => LoggerFactory?.Create("Application") ?? NullLogger.Instance;
	}
}
