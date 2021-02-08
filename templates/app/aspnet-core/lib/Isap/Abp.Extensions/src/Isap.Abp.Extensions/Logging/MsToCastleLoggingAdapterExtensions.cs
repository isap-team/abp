using System;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ILogger = Castle.Core.Logging.ILogger;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Isap.Abp.Extensions.Logging
{
	public static class MsToCastleLoggingAdapterExtensions
	{
		/*
		public static void UseCastleLoggerFactory(this IApplicationBuilder app)
		{
			var castleLoggerFactory = app.ApplicationServices.GetService<Castle.Core.Logging.ILoggerFactory>();
			if (castleLoggerFactory == null)
			{
				return;
			}

			app.ApplicationServices
				.GetRequiredService<ILoggerFactory>()
				.AddCastleLogger(castleLoggerFactory);
		}
		*/

		public static void UseMsToCastleLoggingAdapter(this IServiceCollection services)
		{
			services.AddTransient<ILoggerFactory, MsToCastleLoggingFactory>();
		}
	}

	public class MsToCastleLoggingFactory: ILoggerFactory
	{
		private readonly Microsoft.Extensions.Logging.ILoggerFactory _msLoggerFactory;

		public MsToCastleLoggingFactory(Microsoft.Extensions.Logging.ILoggerFactory msLoggerFactory)
		{
			_msLoggerFactory = msLoggerFactory;
		}

		public ILogger Create(Type type)
		{
			return new MsToCastleLogger(this, _msLoggerFactory.CreateLogger(type), type);
		}

		public ILogger Create(string name)
		{
			return new MsToCastleLogger(this, _msLoggerFactory.CreateLogger(name), name);
		}

		public ILogger Create(Type type, LoggerLevel level)
		{
			return new MsToCastleLogger(this, _msLoggerFactory.CreateLogger(type), type, level);
		}

		public ILogger Create(string name, LoggerLevel level)
		{
			return new MsToCastleLogger(this, _msLoggerFactory.CreateLogger(name), name, level);
		}
	}

	public class MsToCastleLogger: LoggerBase
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly Microsoft.Extensions.Logging.ILogger _msLogger;
		private readonly string _loggerName;

		public MsToCastleLogger(ILoggerFactory loggerFactory, Microsoft.Extensions.Logging.ILogger msLogger, Type type, LoggerLevel? level = null)
		{
			_loggerName = type.FullName;
			_loggerFactory = loggerFactory;
			_msLogger = msLogger;
			Level = level;
		}

		public MsToCastleLogger(ILoggerFactory loggerFactory, Microsoft.Extensions.Logging.ILogger msLogger, string name, LoggerLevel? level = null)
		{
			_loggerName = name;
			_loggerFactory = loggerFactory;
			_msLogger = msLogger;
			Level = level;
		}

		protected LoggerLevel? Level { get; }

		public override bool IsTraceEnabled => _msLogger.IsEnabled(LogLevel.Trace);
		public override bool IsDebugEnabled => _msLogger.IsEnabled(LogLevel.Debug);
		public override bool IsInfoEnabled => _msLogger.IsEnabled(LogLevel.Information);
		public override bool IsWarnEnabled => _msLogger.IsEnabled(LogLevel.Warning);
		public override bool IsErrorEnabled => _msLogger.IsEnabled(LogLevel.Error);
		public override bool IsFatalEnabled => _msLogger.IsEnabled(LogLevel.Critical);

		public override ILogger CreateChildLogger(string loggerName)
		{
			return Level.HasValue
					? _loggerFactory.Create(_loggerName + "." + loggerName, Level.Value)
					: _loggerFactory.Create(_loggerName + "." + loggerName)
				;
		}

		protected override void InternalFormat(LoggerLevel level, Exception exception, Func<string> formatMessage)
		{
			LogLevel msLogLevel = ConvertLogLevel(Level ?? level);
			if (_msLogger.IsEnabled(msLogLevel))
				_msLogger.Log<object>(msLogLevel, new EventId(), null, exception, (state, ex) => formatMessage());
		}

		protected static LogLevel ConvertLogLevel(LoggerLevel level)
		{
			switch (level)
			{
				case LoggerLevel.Fatal:
					return LogLevel.Critical;
				case LoggerLevel.Error:
					return LogLevel.Error;
				case LoggerLevel.Warn:
					return LogLevel.Warning;
				case LoggerLevel.Info:
					return LogLevel.Information;
				case LoggerLevel.Debug:
					return LogLevel.Debug;
				case LoggerLevel.Trace:
					return LogLevel.Trace;
				case LoggerLevel.Off:
					return LogLevel.None;
				default:
					return LogLevel.None;
			}
		}
	}
}
