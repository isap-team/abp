using System.Threading.Tasks;
using Isap.Abp.Extensions.Api.Clients;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Api
{
	public abstract class BasicServiceProxyBase<TEntityDto, TKey, TApiApplication, TApiClient, TApiOptions, TApiManager>
		: ReferencesServiceProxyBase<TEntityDto, TKey, TApiApplication, TApiClient, TApiOptions, TApiManager>, IBasicAppService<TEntityDto, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
		where TApiClient: IAbpApiClient
		where TApiOptions: AbpApiOptions, new()
		where TApiApplication: IAbpApiApplication<TApiClient, TApiOptions>
		where TApiManager: class, IBasicApiManager<TEntityDto, TKey>
	{
		// [HttpPost]
		// [Route("create")]
		public virtual async Task<TEntityDto> Create(TEntityDto entry)
		{
			TEntityDto result = await RemoteCall(session => ApiManager.CreateAsync(session, entry));
			if (result != null)
				await ComplementResult(result);
			return result;
		}

		// [HttpPut]
		// [Route("update")]
		public virtual async Task<TEntityDto> Update(TEntityDto entry)
		{
			TEntityDto result = await RemoteCall(session => ApiManager.UpdateAsync(session, entry));
			if (result != null)
				await ComplementResult(result);
			return result;
		}

		// [HttpPost]
		// [Route("save")]
		public virtual async Task<TEntityDto> Save(TEntityDto entry)
		{
			TEntityDto result = await RemoteCall(session => ApiManager.SaveAsync(session, entry));
			if (result != null)
				await ComplementResult(result);
			return result;
		}

		// [HttpDelete]
		// [Route("delete")]
		public virtual async Task Delete(TKey id)
		{
			await RemoteCall(session => ApiManager.DeleteAsync(session, id));
		}

		// [HttpPost]
		// [Route("undelete")]
		public async Task<TEntityDto> Undelete(TKey id)
		{
			TEntityDto result = await RemoteCall(session => ApiManager.UndeleteAsync(session, id));
			if (result != null)
				await ComplementResult(result);
			return result;
		}
	}
}
