using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Abp.Web.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Isap.Abp.Extensions.Api.Clients
{
	public interface IAbpApiClient
	{
		[NotNull]
		JsonSerializerSettings JsonSerializerSettings { get; }

		[NotNull]
		Uri BaseUrl { get; }

		Task<AjaxResponse<TResponse>> ProcessGetRequestAsync<TResponse>(IAbpApiSession session, string relativeUrl,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default);

		Task<AjaxResponse<TResponse>> ProcessGetRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default);

		Task<AjaxResponse<TResponse>> ProcessPostRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default);

		Task<AjaxResponse<TResponse>> ProcessPutRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default);

		Task<AjaxResponse<TResponse>> ProcessDeleteRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default);
	}
}
