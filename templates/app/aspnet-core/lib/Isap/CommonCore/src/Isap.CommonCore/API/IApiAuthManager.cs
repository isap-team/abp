using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Isap.CommonCore.API
{
	public interface IApiAuthManager
	{
		Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders, Dictionary<string, object> requestParams);
	}
}
