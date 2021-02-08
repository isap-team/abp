using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public static class RequestResponseLoggingMiddlewareExtensions
	{
		public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder, Action<IsapRequestLoggingOptions> setupAction = null)
		{
			IsapRequestLoggingOptions options = builder.ApplicationServices.GetRequiredService<IOptions<IsapRequestLoggingOptions>>().Value;
			setupAction?.Invoke(options);
			return builder.UseMiddleware<LogRequestMiddleware>(options).UseMiddleware<LogResponseMiddleware>(options);
		}

		public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, IsapRequestLoggingOptions options = null)
		{
			IsapRequestLoggingOptions requestLoggingOptions = options ?? new IsapRequestLoggingOptions();
			return builder.UseMiddleware<LogRequestMiddleware>(requestLoggingOptions);
		}

		public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, IsapRequestLoggingOptions options = null)
		{
			IsapRequestLoggingOptions requestLoggingOptions = options ?? new IsapRequestLoggingOptions();
			return builder.UseMiddleware<LogResponseMiddleware>(requestLoggingOptions);
		}
	}
}
