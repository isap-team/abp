using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Isap.CommonCore.API
{
	public abstract class ApiAuthManagerBase: IApiAuthManager
	{
		private readonly ConcurrentDictionary<string, AuthTokenItem> _authTokenMap = new ConcurrentDictionary<string, AuthTokenItem>();

		public abstract Task AuthenticateRequest(ApiRequestContext context, IApiClient client, HttpRequestHeaders requestHeaders,
			Dictionary<string, object> requestParams);

		protected async Task<AuthTokenItem> GetAuthToken([NotNull] string login, Func<Task<AuthTokenItem>> create)
		{
			if (login == null) throw new ArgumentNullException(nameof(login));

			AuthTokenItem item;
			if (_authTokenMap.TryGetValue(login, out item))
			{
				DateTime now = DateTime.UtcNow;
				if (item.ExpiresAt < now)
					item = null;
			}

			if (item == null)
			{
				item = await create();
				item = _authTokenMap.AddOrUpdate(login, dummy => item, (dummy1, dummy2) => item);
			}

			if (item == null)
				throw new InvalidOperationException();

			return item;
		}
	}
}
