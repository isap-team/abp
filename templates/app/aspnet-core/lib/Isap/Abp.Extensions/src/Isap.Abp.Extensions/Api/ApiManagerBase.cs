using System.Net.Http;
using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.Abp.Extensions.Api.Clients;
using Newtonsoft.Json;

namespace Isap.Abp.Extensions.Api
{
	public interface IApiManager
	{
	}

	public abstract class ApiManagerBase<TApiClient>: IApiManager
		where TApiClient: IAbpApiClient
	{
		protected ApiManagerBase(TApiClient client, string serviceBaseUrl)
		{
			Client = client;
			ServiceBaseUrl = serviceBaseUrl;
		}

		public TApiClient Client { get; }
		public string ServiceBaseUrl { get; }

		protected virtual async Task<AjaxResponse<TResponse>> DeserializeResponse<TResponse>(HttpResponseMessage responseMessage)
		{
			string responseJson = await responseMessage.Content.ReadAsStringAsync();
			AjaxResponse<TResponse> response = JsonConvert.DeserializeObject<AjaxResponse<TResponse>>(responseJson, Client.JsonSerializerSettings);
			return response;
		}

		protected async Task<AjaxResponse<TResponse>> ProcessGetAsync<TRequest, TResponse>(IAbpApiSession session, string actionUrl, TRequest request)
		{
			return await Client.ProcessGetRequestAsync<object, TResponse>(session, $"{ServiceBaseUrl}/{actionUrl}", request, false,
				DeserializeResponse<TResponse>);
		}

		protected async Task<AjaxResponse<TResponse>> ProcessPostAsync<TRequest, TResponse>(IAbpApiSession session, string actionUrl, TRequest request)
		{
			return await Client.ProcessPostRequestAsync<object, TResponse>(session, $"{ServiceBaseUrl}/{actionUrl}", request, false,
				DeserializeResponse<TResponse>);
		}

		protected async Task<AjaxResponse<TResponse>> ProcessPutAsync<TRequest, TResponse>(IAbpApiSession session, string actionUrl, TRequest request)
		{
			return await Client.ProcessPutRequestAsync<object, TResponse>(session, $"{ServiceBaseUrl}/{actionUrl}", request, false,
				DeserializeResponse<TResponse>);
		}

		protected async Task<AjaxResponse<TResponse>> ProcessDeleteAsync<TRequest, TResponse>(IAbpApiSession session, string actionUrl, TRequest request)
		{
			return await Client.ProcessDeleteRequestAsync<object, TResponse>(session, $"{ServiceBaseUrl}/{actionUrl}", request, false,
				DeserializeResponse<TResponse>);
		}
	}
}
