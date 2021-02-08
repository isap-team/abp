using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Isap.CommonCore.Caching;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Isap.Converters;
using Isap.Converters.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.API
{
	public abstract class ApiClientBase: IApiClient
	{
		protected static IValueConverter Converter = ValueConverterProviders.Default.GetConverter();
		protected static readonly IApiRequestCache __nullApiRequestCache = new NullApiRequestCache();
		public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(30);

		static ApiClientBase()
		{
			Encoding.RegisterProvider(new CustomEncodingProvider());
		}

		public abstract JsonSerializerSettings JsonSerializerSettings { get; }

		public ILogger Logger { get; set; }

		public virtual ApiRequestContext CreateRequestContext(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ApiRequestContext
				{
					CancellationToken = cancellationToken,
				};
		}

		public ApiRequestContext CreateRequestContext(string apiSubdomain, CancellationToken cancellationToken = new CancellationToken())
		{
			return new ApiRequestContext(CreateRequestContext(cancellationToken), apiSubdomain);
		}

		public ApiRequestContext CreateRequestContext(string apiLogin, string apiPassword,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ApiRequestContext(CreateRequestContext(cancellationToken), apiLogin, apiPassword);
		}

		public ApiRequestContext CreateRequestContext(string apiSubdomain, string apiLogin, string apiPassword,
			CancellationToken cancellationToken = new CancellationToken())
		{
			return new ApiRequestContext(CreateRequestContext(cancellationToken), apiSubdomain, apiLogin, apiPassword);
		}

		public abstract Task<AuthTokenItem> GetAuthToken(ApiRequestContext context);
	}

	public interface IApiWrappedResponse<TResponse, TResponseBase>
	{
		TResponse Response { get; }
		TResponseBase ResponseBase { get; }
	}

	public abstract class ApiClientBase<TResponseBase>: ApiClientBase
		where TResponseBase: class
	{
		private IApiRequestCache _cache;

		protected ApiClientBase(string baseUrl, IApiAuthManager apiAuthManager)
		{
			IgnoreSslErrors = false;
			BaseUrl = baseUrl;
			ApiAuthManager = apiAuthManager;
		}

		public string BaseUrl { get; }
		public IApiAuthManager ApiAuthManager { get; }

		public bool IgnoreSslErrors { get; set; }

		public IApiRequestCache Cache
		{
			get => _cache ?? __nullApiRequestCache;
			set => _cache = value;
		}

		public IWebProxy WebProxy { get; set; }

		public abstract string UserAgentName { get; }

		protected Task<TWrappedResponse> ProcessGetRequestAsync<TRequest, TResponse, TWrappedResponse>(ApiRequestContext context, string url, TRequest request)
			where TRequest: class
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			return ProcessRequestAsync<TRequest, TResponse, TWrappedResponse>(context, url, request,
				(client, uri, content) => client.GetAsync(uri, context.CancellationToken));
		}

		protected Task<TWrappedResponse> ProcessPostRequestAsync<TRequest, TResponse, TWrappedResponse>(ApiRequestContext context, string url, TRequest request)
			where TRequest: class
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			return ProcessRequestAsync<TRequest, TResponse, TWrappedResponse>(context, url, request,
				(client, uri, content) => client.PostAsync(uri, content, context.CancellationToken));
		}

		protected Task<TWrappedResponse> ProcessPutRequestAsync<TRequest, TResponse, TWrappedResponse>(ApiRequestContext context, string url, TRequest request)
			where TRequest: class
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			return ProcessRequestAsync<TRequest, TResponse, TWrappedResponse>(context, url, request,
				(client, uri, content) => client.PutAsync(uri, content, context.CancellationToken));
		}

		protected Task<TWrappedResponse> ProcessDeleteRequestAsync<TRequest, TResponse, TWrappedResponse>(ApiRequestContext context, string url,
			TRequest request)
			where TRequest: class
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			return ProcessRequestAsync<TRequest, TResponse, TWrappedResponse>(context, url, request,
				(client, uri, content) => client.DeleteAsync(uri, context.CancellationToken));
		}

		protected async Task<TWrappedResponse> ProcessRequestAsync<TRequest, TResponse, TWrappedResponse>(ApiRequestContext context, string url,
			TRequest request, Func<HttpClient, Uri, HttpContent, Task<HttpResponseMessage>> getResponseAsync)
			where TRequest: class
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			var handler = new HttpClientHandler
				{
					UseCookies = true,
					CookieContainer = context.Cookies,
					Proxy = WebProxy,
					UseDefaultCredentials = WebProxy?.Credentials != null,
				};

			if (IgnoreSslErrors)
				handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

			using (var client = new HttpClient(handler))
			{
				if (context.CancellationToken == CancellationToken.None)
					client.Timeout = context.RequestTimeout.IsDefaultValue() ? DefaultRequestTimeout : context.RequestTimeout;

				client.DefaultRequestHeaders.Add("User-Agent", UserAgentName);

				object ApiTranslateValue(ApiCollectionJoinAttribute attr, object value)
				{
					if (attr == null) return value;

					switch (value)
					{
						case ICollection collection:
							return string.Join(attr.Delimiter, collection.Cast<object>().Select(i => i?.ToString()));
						default:
							return value;
					}
				}

				void AssignRequestHeader(ApiRequestHeaderAttribute attr, object value)
				{
					if (attr == null) return;

					try
					{
						// ReSharper disable once AccessToDisposedClosure
						client.DefaultRequestHeaders.Add(attr.Name, string.Format(attr.Format, value));
					}
					catch (FormatException)
					{
						//TODO: add warning to log.
					}
				}

				void OnAssignMember(AssignMemberState state)
				{
					if (state.Value != null)
					{
						state.Value = ApiTranslateValue(state.Member.GetCustomAttribute<ApiCollectionJoinAttribute>(), state.Value);

						AssignRequestHeader(state.Member.GetCustomAttribute<ApiRequestHeaderAttribute>(), state.Value);
					}
				}

				object wrappedRequest = WrapRequest(context, request);

				Dictionary<string, object> requestParams = null;
				object requestBody;
				switch (wrappedRequest)
				{
					case null:
						requestBody = null;
						break;
					case string strRequest:
						requestBody = strRequest;
						break;
					case ICollection collection:
						requestBody = collection.AsNameToObjectMapTree(onAssignMember: OnAssignMember);
						break;
					default:
						requestParams = wrappedRequest.AsNameToObjectMap(deep: true, ignoreDefaults: true, onAssignMember: OnAssignMember);
						requestBody = requestParams;
						break;
				}

				requestParams = requestParams ?? new Dictionary<string, object>();

				if (context.RequiresAuthentication)
				{
					await ApiAuthManager.AuthenticateRequest(context, this, client.DefaultRequestHeaders, requestParams);
				}

				string queryString = null;
				HttpContent contentData = null;
				switch (context.RequestBodyType)
				{
					case ApiRequestBodyType.None:
						queryString = GetQueryString(requestParams);
						break;
					case ApiRequestBodyType.JsonQueryString:
						queryString = GetJsonQueryString(requestParams);
						break;
					default:
						contentData = GetContentData(context.RequestBodyType, requestBody);
						break;
				}

				if (!String.IsNullOrEmpty(queryString))
				{
					url += url.IndexOf('?') < 0 ? "?" : "&";
					url += queryString;
				}

				var requestUri = new Uri(url, UriKind.RelativeOrAbsolute);
				if (!requestUri.IsAbsoluteUri)
					requestUri = new Uri(new Uri(string.Format(BaseUrl, context.ApiSubdomain), UriKind.Absolute), url);

				string queryData = contentData == null ? queryString : await contentData.ReadAsStringAsync();

				/*
				var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
					{
						Content = contentData,
					};

				HttpResponseMessage responseMessage = await client.SendAsync(requestMessage, context.CancellationToken);
				*/
				HttpResponseMessage responseMessage = await getResponseAsync(client, requestUri, contentData);

				context.CancellationToken.ThrowIfCancellationRequested();
				if (Logger != null)
				{
					LogRequest(queryData, requestUri.AbsoluteUri);
					await LogResponse(responseMessage);
				}

				return await DeserializeResponse<TResponse, TWrappedResponse>(context, responseMessage);
			}
		}

		protected virtual object WrapRequest<TRequest>(ApiRequestContext context, TRequest request)
			where TRequest: class
		{
			return request;
		}

		protected virtual TWrappedResponse WrapResponse<TResponse, TWrappedResponse>(ApiRequestContext context, HttpResponseMessage responseMessage,
			object response)
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			switch (response)
			{
				case TWrappedResponse typedResponse:
					return typedResponse;
				case JToken token:
					return token.ToObject<TWrappedResponse>(JsonSerializer.Create(JsonSerializerSettings));
				default:
					throw new InvalidOperationException();
			}
		}

		protected virtual HttpContent GetContentData(ApiRequestBodyType requestBodyType, object request)
		{
			switch (requestBodyType)
			{
				case ApiRequestBodyType.Json:
					return GetJsonContent(request);
				case ApiRequestBodyType.FormData:
					return GetFormDataContent(request);
				case ApiRequestBodyType.PhpHttpQuery:
					return GetPhpHttpQueryContent(request);
				default:
					throw new NotSupportedException();
			}
		}

		protected virtual async Task<TWrappedResponse> DeserializeResponse<TResponse, TWrappedResponse>(ApiRequestContext context,
			HttpResponseMessage responseMessage)
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			if (responseMessage.StatusCode == HttpStatusCode.NoContent)
				return new TWrappedResponse();

			string responseJson = await responseMessage.Content.ReadAsStringAsync();
			JToken jToken = JToken.Parse(responseJson, new JsonLoadSettings
				{
					CommentHandling = CommentHandling.Ignore,
					LineInfoHandling = LineInfoHandling.Ignore,
				});

			if (responseMessage.IsSuccessStatusCode)
				return WrapResponse<TResponse, TWrappedResponse>(context, responseMessage, jToken);

			return ProcessUnsuccessResponse<TResponse, TWrappedResponse>(context, responseMessage, jToken);
		}

		protected virtual TWrappedResponse ProcessUnsuccessResponse<TResponse, TWrappedResponse>(ApiRequestContext context, HttpResponseMessage responseMessage,
			JToken jToken)
			where TResponse: class
			where TWrappedResponse: class, IApiWrappedResponse<TResponse, TResponseBase>, new()
		{
			if (context.ThrowIfError)
				throw new CommonApiException(responseMessage.StatusCode, responseMessage.ReasonPhrase);
			return default(TWrappedResponse);
		}

		private string GetQueryString(Dictionary<string, object> request)
		{
			return String.Join("&", request.Select(t => WebUtility.UrlEncode(t.Key) + '=' + WebUtility.UrlEncode(t.Value.ToStringOrNull())));
		}

		private string GetJsonQueryString(Dictionary<string, object> request)
		{
			return WebUtility.UrlEncode(JsonConvert.SerializeObject(request, JsonSerializerSettings));
		}

		private StringContent GetJsonContent(object request)
		{
			string requestData = JsonConvert.SerializeObject(request, JsonSerializerSettings);
			return new StringContent(requestData, Encoding.UTF8, "application/json");
		}

		private StringContent GetFormDataContent(object request)
		{
			switch (request)
			{
				case Dictionary<string, object> map:
					string requestData = String.Join("&", map.Select(t => WebUtility.UrlEncode(t.Key) + '=' + WebUtility.UrlEncode(t.Value.ToStringOrNull())));
					return new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
				default:
					throw new InvalidOperationException();
			}
		}

		private StringContent GetPhpHttpQueryContent(object request)
		{
			switch (request)
			{
				case Dictionary<string, object> map:
					string requestData = String.Join("&",
						map.ToPhpHttpQuery().Select(t => WebUtility.UrlEncode(t.Item1) + '=' + WebUtility.UrlEncode(t.Item2)));
					return new StringContent(requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
				default:
					throw new InvalidOperationException();
			}
		}

		private void LogRequest(string queryData, string url)
		{
			string message = FormatMessage(msg =>
				{
					msg.AppendLine($"URL: {url}");
					msg.AppendLine(queryData);
				});

			Logger.Debug($"HTTP REQUEST: {message}");
		}

		private async Task LogResponse(HttpResponseMessage response)
		{
			var responseBody = await response.Content.ReadAsStringAsync();

			string message = FormatMessage(msg =>
				{
					msg.AppendLine($"Method: {response.RequestMessage.Method}");
					msg.AppendLine($"URL: {response.RequestMessage.RequestUri}");
					msg.AppendLine(responseBody);
				});

			Logger.Debug($"HTTP RESPONSE ({response.StatusCode}): {message}");
		}

		private string FormatMessage(Action<StringBuilder> format)
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
