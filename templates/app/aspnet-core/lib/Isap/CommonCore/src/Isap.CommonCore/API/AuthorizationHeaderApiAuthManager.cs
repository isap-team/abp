using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Isap.CommonCore.API
{
	public class AuthorizationHeaderApiAuthManager: ApiAuthManagerBase
	{
		public AuthorizationHeaderApiAuthManager(string scheme)
		{
			Scheme = scheme;
		}

		public string Scheme { get; }

		public override async Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders,
			Dictionary<string, object> requestParams)
		{
			AuthTokenItem authTokenItem = await GetAuthToken(context.ApiLogin, () => client.GetAuthToken(context));
			requestHeaders.Authorization = new AuthenticationHeaderValue(Scheme, authTokenItem.AuthToken);
		}
	}
}
