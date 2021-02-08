using System;
using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.Abp.Extensions.Api.Clients;
using Microsoft.Extensions.Options;
using Volo.Abp;

namespace Isap.Abp.Extensions.Api
{
	public abstract class ServiceProxyBase<TApiApplication, TApiClient, TApiOptions>: DisposableDomainServiceBase
		where TApiClient: IAbpApiClient
		where TApiOptions: AbpApiOptions, new()
		where TApiApplication: IAbpApiApplication<TApiClient, TApiOptions>
	{
		private readonly Lazy<TApiClient> _lazyApiClient;

		protected ServiceProxyBase()
		{
			_lazyApiClient = new Lazy<TApiClient>(() => ApiApplication.CreateClient(ApiOptions));
		}

		protected TApiOptions ApiOptions => LazyGetRequiredService<IOptions<TApiOptions>>().Value;
		protected TApiApplication ApiApplication => LazyGetRequiredService<TApiApplication>();
		protected TApiClient ApiClient => _lazyApiClient.Value;

		protected override void InternalDispose()
		{
			if (_lazyApiClient.IsValueCreated)
				ApiApplication.Release(_lazyApiClient.Value);
		}

		protected async Task<TDto> RemoteCall<TDto>(Func<IAbpApiSession, Task<AjaxResponse<TDto>>> action, bool requiresAuth = true)
		{
			IAbpApiSession session = ApiApplication.CreateSession();
			if (requiresAuth)
				await AuthenticateRequest(session);
			AjaxResponse<TDto> response = await action(session);
			CheckError(response);
			return response.Result;
		}

		protected async Task RemoteCall(Func<IAbpApiSession, Task<AjaxResponse>> action, bool requiresAuth = true)
		{
			IAbpApiSession session = ApiApplication.CreateSession();
			if (requiresAuth)
				await AuthenticateRequest(session);
			AjaxResponse response = await action(session);
			CheckError(response);
		}

		protected abstract Task AuthenticateRequest(IAbpApiSession session);

		private static void CheckError(AjaxResponseBase response)
		{
			if (!response.Success)
			{
				AbpIoResponse abpIoResponse = response.ToAbpIoResponse();
				if (abpIoResponse != null)
				{
					if (!int.TryParse(abpIoResponse.Error.Code, out int code))
						code = -1;
					throw new UserFriendlyException(abpIoResponse.Error.Message, code.ToString(), abpIoResponse.Error.Details);
				}

				throw new UserFriendlyException(response.Error.Message, response.Error.Code.ToString(), response.Error.Details);
			}
		}
	}

	public abstract class ServiceProxyBase<TApiApplication, TApiClient, TApiOptions, TApiManager>
		: ServiceProxyBase<TApiApplication, TApiClient, TApiOptions>
		where TApiClient: IAbpApiClient
		where TApiOptions: AbpApiOptions, new()
		where TApiApplication: IAbpApiApplication<TApiClient, TApiOptions>
		where TApiManager: class, IApiManager
	{
		private readonly Lazy<TApiManager> _lazyApiManager;

		protected ServiceProxyBase()
		{
			_lazyApiManager = new Lazy<TApiManager>(CreateApiManager);
		}

		protected TApiManager ApiManager => _lazyApiManager.Value;

		protected abstract TApiManager CreateApiManager();
	}
}
