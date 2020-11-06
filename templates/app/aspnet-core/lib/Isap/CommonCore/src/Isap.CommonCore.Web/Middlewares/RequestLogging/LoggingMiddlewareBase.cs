using System;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
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
			Logger = loggerFactory.Create("RequestLogging");
		}

		public IsapRequestLoggingOptions Options { get; }

		protected ILogger Logger { get; }

		public virtual Task Invoke(HttpContext context)
		{
			if (Options.IsRequestLoggingEnabled(context.Request.Path))
				return InternalInvoke(context);
			return _next(context);
		}

		protected virtual Task InternalInvoke(HttpContext context)
		{
			return _next(context);
		}

		protected void Log(string message)
		{
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
