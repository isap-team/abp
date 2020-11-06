using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Isap.CommonCore.Extensions;

namespace Isap.CommonCore.API
{
	public class QueryParameterApiAuthManager: ApiAuthManagerBase
	{
		public override Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders,
			Dictionary<string, object> requestParams)
		{
			requestParams["login"] = context.ApiLogin;
			requestParams["psw"] = context.ApiPassword;
			requestParams["fmt"] = 3;
			requestParams["cost"] = context.Properties.GetOrDefault("cost", 2);
			return Task.CompletedTask;
		}
	}
}
