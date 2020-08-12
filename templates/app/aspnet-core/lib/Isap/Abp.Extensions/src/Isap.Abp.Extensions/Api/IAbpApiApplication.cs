using System;
using Isap.Abp.Extensions.Api.Clients;
using Microsoft.Extensions.Options;

namespace Isap.Abp.Extensions.Api
{
	public interface IAbpApiApplication
	{
	}

	public interface IAbpApiApplication<TApiClient, in TApiOptions>
		where TApiClient: IAbpApiClient
		where TApiOptions: AbpApiOptions, new()
	{
		IAbpApiSession CreateSession();
		IAbpApiSession CreateSession(int nodeKey);

		TApiClient CreateClient(IOptions<TApiOptions> options);
		TApiClient CreateClient(Uri baseUrl);
		TApiClient CreateClient(string baseUrl);
		void Release(TApiClient client);
	}
}
