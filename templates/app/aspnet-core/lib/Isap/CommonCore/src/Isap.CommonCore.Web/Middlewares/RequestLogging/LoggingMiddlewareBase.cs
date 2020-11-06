using System;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Isap.CommonCore.Logging;
using Microsoft.AspNetCore.Http;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public abstract class LoggingMiddlewareBase
	{
		private readonly RequestDelegate _next;

		protected LoggingMiddlewareBase(RequestDelegate next, ILoggerFactory loggerFactory, IsapRequestLoggingOptions options)
		{
			Options = options;
			_next = next;
			Logger = loggerFactory.Create(GetType());
		}

		public IsapRequestLoggingOptions Options { get; }

		protected ILogger Logger { get; }

		public virtual async Task Invoke(HttpContext context)
		{
			if (Options.IsRequestLoggingEnabled(context.Request.Path))
			{
				await InternalInvoke(context);
			}
			else
				await _next(context);
		}

		protected virtual Task InternalInvoke(HttpContext context)
		{
			return _next(context);
		}

		protected void Log(string message)
		{
			using (LoggingContext.Current.WithLogicalProperty(Logger, "LoggingArea", "RequestLogging"))
			using (LoggingContext.Current.WithLogicalProperty(Logger, "LoggingSource", GetType().Name))
				Logger.Info(message);
		}

		protected string FormatMessage(Action<StringBuilder> format)
		{
			var msg = new StringBuilder();
			msg.AppendLine();
			msg.Append(new string('<', 10)).AppendLine(new string('-', 70));
			format(msg);
			msg.Append(new string('-', 70)).AppendLine(new string('>', 10));
			return msg.ToString();
		}
	}
}
