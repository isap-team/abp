using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Isap.CommonCore.Logging;

namespace Isap.CommonCore.Web.Middlewares.Tracing
{
	public class LoggingTraceIdentifierMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILoggerFactory _loggerFactory;

		public LoggingTraceIdentifierMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			_loggerFactory = loggerFactory;
		}

		public Task Invoke(HttpContext context)
		{
			ILogger logger = _loggerFactory.Create(typeof(LoggingTraceIdentifierMiddleware));
			using (LoggingContext.Current.WithLogicalProperty(logger, "TraceId", context.TraceIdentifier))
				return _next(context);
		}
	}
}
