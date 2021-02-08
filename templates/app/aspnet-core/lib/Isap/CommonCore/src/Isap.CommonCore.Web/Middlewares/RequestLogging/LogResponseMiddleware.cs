using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public class LogResponseMiddleware: LoggingMiddlewareBase
	{
		public LogResponseMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IsapRequestLoggingOptions options)
			: base(next, loggerFactory, options)
		{
		}

		protected override async Task InternalInvoke(HttpContext context)
		{
			var bodyStream = context.Response.Body;

			var responseBodyStream = new MemoryStream();
			context.Response.Body = responseBodyStream;

			await base.InternalInvoke(context);

			// иногда responseBodyStream что-то на пути обработки запроса закрывает
			if (responseBodyStream.CanSeek)
				responseBodyStream.Seek(0, SeekOrigin.Begin);
			else
				responseBodyStream = new MemoryStream(responseBodyStream.ToArray());

			Log(context.Response, responseBodyStream);
			if (responseBodyStream.Length > 0)
			{
				responseBodyStream.Seek(0, SeekOrigin.Begin);
				await responseBodyStream.CopyToAsync(bodyStream);
			}

			context.Response.Body = bodyStream;
		}

		protected void Log(HttpResponse response, Stream responseBodyStream)
		{
			var responseBody = new StreamReader(responseBodyStream).ReadToEnd();

			string message = FormatMessage(msg =>
				{
					foreach (KeyValuePair<string, StringValues> header in response.Headers)
						msg.AppendLine($"{header.Key}: {header.Value}");
					msg.AppendLine();
					msg.AppendLine(responseBody);
				});

			Log($"HTTP RESPONSE ({response.StatusCode}): {message}");
		}
	}
}
