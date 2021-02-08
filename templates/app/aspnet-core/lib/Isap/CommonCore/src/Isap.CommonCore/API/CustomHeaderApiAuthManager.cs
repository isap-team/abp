using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Isap.CommonCore.API
{
	public class CustomHeaderApiAuthManager: ApiAuthManagerBase
	{
		public CustomHeaderApiAuthManager(string headerName)
		{
			HeaderName = headerName;
		}

		public string HeaderName { get; }

		public override async Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders,
			Dictionary<string, object> requestParams)
		{
			AuthTokenItem authTokenItem = await GetAuthToken(context.ApiLogin, () => client.GetAuthToken(context));
			requestHeaders.Add(HeaderName, authTokenItem.AuthToken);
		}
	}
}
