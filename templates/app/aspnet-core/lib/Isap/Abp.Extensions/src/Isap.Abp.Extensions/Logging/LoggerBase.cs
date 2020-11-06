using System;
using Castle.Core.Logging;

namespace Isap.Abp.Extensions.Logging
{
	public abstract class LoggerBase: ILogger
	{
		public abstract bool IsTraceEnabled { get; }
		public abstract bool IsDebugEnabled { get; }
		public abstract bool IsInfoEnabled { get; }
		public abstract bool IsWarnEnabled { get; }
		public abstract bool IsErrorEnabled { get; }
		public abstract bool IsFatalEnabled { get; }

		public abstract ILogger CreateChildLogger(string loggerName);

		protected abstract void InternalFormat(LoggerLevel level, Exception exception, Func<string> formatMessage);

		#region Trace

		public void Trace(string message)
		{
			InternalFormat(LoggerLevel.Trace, null, () => message);
		}

		public void Trace(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Trace, null, messageFactory);
		}

		public void Trace(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Trace, exception, () => message);
		}

		public void TraceFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Trace, null, () => string.Format(format, args));
		}

		public void TraceFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Trace, exception, () => string.Format(format, args));
		}

		public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Trace, null, () => string.Format(formatProvider, format, args));
		}

		public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Trace, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion

		#region Debug

		public void Debug(string message)
		{
			InternalFormat(LoggerLevel.Debug, null, () => message);
		}

		public void Debug(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Debug, null, messageFactory);
		}

		public void Debug(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Debug, exception, () => message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Debug, null, () => string.Format(format, args));
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Debug, exception, () => string.Format(format, args));
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Debug, null, () => string.Format(formatProvider, format, args));
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Debug, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion

		#region Error

		public void Error(string message)
		{
			InternalFormat(LoggerLevel.Error, null, () => message);
		}

		public void Error(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Error, null, messageFactory);
		}

		public void Error(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Error, exception, () => message);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Error, null, () => string.Format(format, args));
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Error, exception, () => string.Format(format, args));
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Error, null, () => string.Format(formatProvider, format, args));
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Error, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion

		#region Fatal

		public void Fatal(string message)
		{
			InternalFormat(LoggerLevel.Fatal, null, () => message);
		}

		public void Fatal(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Fatal, null, messageFactory);
		}

		public void Fatal(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Fatal, exception, () => message);
		}

		public void FatalFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Fatal, null, () => string.Format(format, args));
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Fatal, exception, () => string.Format(format, args));
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Fatal, null, () => string.Format(formatProvider, format, args));
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Fatal, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion

		#region Info

		public void Info(string message)
		{
			InternalFormat(LoggerLevel.Info, null, () => message);
		}

		public void Info(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Info, null, messageFactory);
		}

		public void Info(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Info, exception, () => message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Info, null, () => string.Format(format, args));
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Info, exception, () => string.Format(format, args));
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Info, null, () => string.Format(formatProvider, format, args));
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Info, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion

		#region Warn

		public void Warn(string message)
		{
			InternalFormat(LoggerLevel.Warn, null, () => message);
		}

		public void Warn(Func<string> messageFactory)
		{
			InternalFormat(LoggerLevel.Warn, null, messageFactory);
		}

		public void Warn(string message, Exception exception)
		{
			InternalFormat(LoggerLevel.Warn, exception, () => message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Warn, null, () => string.Format(format, args));
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Warn, exception, () => string.Format(format, args));
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Warn, null, () => string.Format(formatProvider, format, args));
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			InternalFormat(LoggerLevel.Warn, exception, () => string.Format(formatProvider, format, args));
		}

		#endregion
	}
}
