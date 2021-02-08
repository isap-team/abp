using System.Threading.Tasks;
using Castle.Core.Logging;
using Isap.CommonCore.Logging;
using Microsoft.AspNetCore.Http;

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

		public async Task Invoke(HttpContext context)
		{
			ILogger logger = _loggerFactory.Create(typeof(LoggingTraceIdentifierMiddleware));
			using (LoggingContext.Current.WithLogicalProperty(logger, "TraceId", context.TraceIdentifier))
			{
				await _next(context);
			}
		}
	}
}
