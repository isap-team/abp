using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Isap.CommonCore.API
{
	public class CookieApiAuthManager: ApiAuthManagerBase
	{
		public CookieApiAuthManager(bool controlledByRemoteServer)
		{
			ControlledByRemoteServer = controlledByRemoteServer;
		}

		public bool ControlledByRemoteServer { get; }

		public override async Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders,
			Dictionary<string, object> requestParams)
		{
			if (ControlledByRemoteServer)
			{
				await GetAuthToken(context.ApiLogin, () => client.GetAuthToken(context));
				return;
			}

			throw new NotImplementedException();
		}
	}
}
