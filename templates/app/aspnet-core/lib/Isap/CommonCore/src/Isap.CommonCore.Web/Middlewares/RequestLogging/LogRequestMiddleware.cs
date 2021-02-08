using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public class LogRequestMiddleware: LoggingMiddlewareBase
	{
		public LogRequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IsapRequestLoggingOptions options)
			: base(next, loggerFactory, options)
		{
		}

		protected override async Task InternalInvoke(HttpContext context)
		{
			if (string.Equals(context.Request.Method, HttpMethods.Get, StringComparison.OrdinalIgnoreCase))
			{
				Log(context.Request, null);

				await base.InternalInvoke(context);
			}
			else
			{
				var requestBodyStream = new MemoryStream();
				var originalRequestBodyStream = context.Request.Body;

				await originalRequestBodyStream.CopyToAsync(requestBodyStream);

				Log(context.Request, requestBodyStream);

				requestBodyStream.Seek(0, SeekOrigin.Begin);
				context.Request.Body = requestBodyStream;

				await base.InternalInvoke(context);
				context.Request.Body = originalRequestBodyStream;
			}
		}

		protected void Log(HttpRequest request, Stream requestBodyStream)
		{
			string responseBody = null;
			if (requestBodyStream != null)
			{
				requestBodyStream.Seek(0, SeekOrigin.Begin);
				responseBody = new StreamReader(requestBodyStream).ReadToEnd();
			}

			string message = FormatMessage(msg =>
				{
					msg.AppendLine($"{request.Method} {request.GetEncodedPathAndQuery()} {request.Protocol}");
					foreach (KeyValuePair<string, StringValues> header in request.Headers)
						msg.AppendLine($"{header.Key}: {header.Value}");
					if (!string.IsNullOrEmpty(responseBody))
					{
						msg.AppendLine();
						msg.AppendLine(responseBody);
					}
				});

			Log($"HTTP REQUEST: {message}");
		}
	}
}
