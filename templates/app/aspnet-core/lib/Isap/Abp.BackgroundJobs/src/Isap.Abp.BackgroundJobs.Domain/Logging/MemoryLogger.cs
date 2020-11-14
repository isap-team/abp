using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Timing;

namespace Isap.Abp.BackgroundJobs.Logging
{
	public class MemoryLogger: ILogger
	{
		private readonly ILogger _parentLogger;
		private readonly List<string> _lines = new List<string>();

		public MemoryLogger(ILogger parentLogger, IClock clock)
		{
			_parentLogger = parentLogger;
			Clock = clock;
		}

		public IClock Clock { get; }

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (IsEnabled(logLevel))
			{
				_lines.Add(string.Format(CultureInfo.InvariantCulture, "{1:5} [{2}] {3}{0}{4}",
					Environment.NewLine, Format(logLevel), Clock.Now, formatter(state, exception), exception));
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return _parentLogger.IsEnabled(logLevel);
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return _parentLogger.BeginScope(state);
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, _lines);
		}

		private string Format(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Trace:
					return "TRACE";
				case LogLevel.Debug:
					return "DEBUG";
				case LogLevel.Information:
					return "INFO";
				case LogLevel.Warning:
					return "WARN";
				case LogLevel.Error:
					return "ERROR";
				case LogLevel.Critical:
					return "FATAL";
				case LogLevel.None:
					return "NONE";
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
