using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.CommonCore.API;
using Isap.CommonCore.Configuration;
using Isap.CommonCore.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Isap.Abp.Extensions.Api.Clients
{
	public abstract class AbpApiClientBase: IAbpApiClient
	{
		private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = new List<JsonConverter>
					{
						new DateTimeJsonConverter(true, null),
					},
			};

		protected AbpApiClientBase([NotNull] Uri baseUrl)
		{
			BaseUrl = baseUrl;
		}

		public abstract string UserAgentName { get; }
		public virtual JsonSerializerSettings JsonSerializerSettings => _jsonSerializerSettings;

		public Uri BaseUrl { get; }
		public IProxyConfiguration ProxyConfig { get; set; }

		public IWebProxy Proxy => ProxyConfig?.GetWebProxy() ?? WebRequest.DefaultWebProxy;

		public Task<AjaxResponse<TResponse>> ProcessGetRequestAsync<TResponse>(IAbpApiSession session, string relativeUrl,
			bool requireAuth, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default)
		{
			if (requireAuth && !session.IsAuthenticated)
				throw new InvalidOperationException("Authentication is required.");

			return ProcessRequestAsync(session, client =>
				{
					var url = new Uri(BaseUrl, relativeUrl);
					return client.GetAsync(url, cancellationToken);
				}, deserializeResponse);
		}

		public Task<AjaxResponse<TResponse>> ProcessGetRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default)
		{
			if (requireAuth && !session.IsAuthenticated)
				throw new InvalidOperationException("Authentication is required.");

			return ProcessRequestAsync(session, client =>
				{
					var url = new Uri(BaseUrl, relativeUrl).SetQueryString(request, false);
					return client.GetAsync(url, cancellationToken);
				}, deserializeResponse);
		}

		public Task<AjaxResponse<TResponse>> ProcessPostRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default)
		{
			if (requireAuth && !session.IsAuthenticated)
				throw new InvalidOperationException("Authentication is required.");

			return ProcessRequestAsync(session, async client =>
				{
					var url = new Uri(BaseUrl, relativeUrl);
					string requestData = JsonConvert.SerializeObject(request, JsonSerializerSettings);
					using (var contentData = new StringContent(requestData, Encoding.UTF8, "application/json"))
						return await client.PostAsync(url, contentData, cancellationToken);
				}, deserializeResponse);
		}

		public Task<AjaxResponse<TResponse>> ProcessPutRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default)
		{
			if (requireAuth && !session.IsAuthenticated)
				throw new InvalidOperationException("Authentication is required.");

			return ProcessRequestAsync(session, async client =>
				{
					var url = new Uri(BaseUrl, relativeUrl);
					string requestData = JsonConvert.SerializeObject(request, JsonSerializerSettings);
					using (var contentData = new StringContent(requestData, Encoding.UTF8, "application/json"))
						return await client.PutAsync(url, contentData, cancellationToken);
				}, deserializeResponse);
		}

		public Task<AjaxResponse<TResponse>> ProcessDeleteRequestAsync<TRequest, TResponse>(IAbpApiSession session, string relativeUrl, TRequest request,
			bool requireAuth = true, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse = null,
			CancellationToken cancellationToken = default)
		{
			if (requireAuth && !session.IsAuthenticated)
				throw new InvalidOperationException("Authentication is required.");

			return ProcessRequestAsync(session, client =>
				{
					var url = new Uri(BaseUrl, relativeUrl).SetQueryString(request, false);
					return client.DeleteAsync(url, cancellationToken);
				}, deserializeResponse);
		}

		private async Task<AjaxResponse<TResponse>> ProcessRequestAsync<TResponse>(IAbpApiSession session,
			Func<HttpClient, Task<HttpResponseMessage>> performRequest, Func<HttpResponseMessage, Task<AjaxResponse<TResponse>>> deserializeResponse)
		{
			var cookieContainer = new CookieContainer();
			var handler = new HttpClientHandler
				{
					UseCookies = true,
					CookieContainer = cookieContainer,
					Proxy = Proxy,
				};

			using (var client = new HttpClient(handler))
			{
#if DEBUG
				client.Timeout = TimeSpan.FromSeconds(3600);
#endif
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Add("User-Agent", UserAgentName);
				if (session.NodeKey.HasValue)
					client.DefaultRequestHeaders.Add("X-NodeKey", session.NodeKey.Value.ToString());
				if (session.AuthToken != null)
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AuthToken.Data);
				if (session.RequestHeaders != null && session.RequestHeaders.Count > 0)
					foreach (KeyValuePair<string, string> pair in session.RequestHeaders)
						client.DefaultRequestHeaders.Add(pair.Key, pair.Value);

				foreach (var cookie in session.Cookies)
					cookieContainer.Add(BaseUrl, cookie);

				deserializeResponse = deserializeResponse ?? DeserializeResponse<TResponse>;

				using (HttpResponseMessage responseMessage = await performRequest(client))
				{
					if (responseMessage.IsSuccessStatusCode)
						return await deserializeResponse(responseMessage);
					string details = await responseMessage.Content.ReadAsStringAsync();
					try
					{
						if (string.IsNullOrEmpty(details))
							return new AjaxResponse<TResponse>(new ErrorInfo((int) responseMessage.StatusCode, responseMessage.ReasonPhrase, details));
						var ajaxResponse = JsonConvert.DeserializeObject<AjaxResponse>(details, JsonSerializerSettings)
							?? throw new InvalidOperationException();
						return new AjaxResponse<TResponse>(ajaxResponse.Error, ajaxResponse.UnAuthorizedRequest);
					}
					catch (Exception)
					{
						return new AjaxResponse<TResponse>(new ErrorInfo((int) responseMessage.StatusCode, responseMessage.ReasonPhrase, details));
					}
				}
			}
		}

		protected async Task<AjaxResponse<TResponse>> DeserializeResponse<TResponse>(HttpResponseMessage responseMessage)
		{
			string responseJson = await responseMessage.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<AjaxResponse<TResponse>>(responseJson, JsonSerializerSettings);
		}
	}
}
