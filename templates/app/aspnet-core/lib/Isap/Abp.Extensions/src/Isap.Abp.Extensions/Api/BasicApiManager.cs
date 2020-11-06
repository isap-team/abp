using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.Abp.Extensions.Api.Clients;

namespace Isap.Abp.Extensions.Api
{
	public interface IBasicApiManager<TEntityDto, TKey>: IReferencesApiManager<TEntityDto, TKey>
	{
		Task<AjaxResponse<TEntityDto>> CreateAsync(IAbpApiSession session, TEntityDto entry);
		Task<AjaxResponse<TEntityDto>> UpdateAsync(IAbpApiSession session, TEntityDto entry);
		Task<AjaxResponse<TEntityDto>> SaveAsync(IAbpApiSession session, TEntityDto entry);
		Task<AjaxResponse> DeleteAsync(IAbpApiSession session, TKey id);
		Task<AjaxResponse<TEntityDto>> UndeleteAsync(IAbpApiSession session, TKey id);
	}

	public abstract class BasicApiManager<TEntityDto, TKey, TApiClient>: ReferenceApiManager<TEntityDto, TKey, TApiClient>, IBasicApiManager<TEntityDto, TKey>
		where TApiClient: IAbpApiClient
	{
		protected BasicApiManager(TApiClient client, string serviceBaseUrl)
			: base(client, serviceBaseUrl)
		{
		}

		public async Task<AjaxResponse<TEntityDto>> CreateAsync(IAbpApiSession session, TEntityDto entry)
		{
			return await ProcessPostAsync<TEntityDto, TEntityDto>(session, $"create", entry);
		}

		public async Task<AjaxResponse<TEntityDto>> UpdateAsync(IAbpApiSession session, TEntityDto entry)
		{
			return await ProcessPutAsync<TEntityDto, TEntityDto>(session, $"update", entry);
		}

		public async Task<AjaxResponse<TEntityDto>> SaveAsync(IAbpApiSession session, TEntityDto entry)
		{
			return await ProcessPostAsync<TEntityDto, TEntityDto>(session, $"save", entry);
		}

		public async Task<AjaxResponse> DeleteAsync(IAbpApiSession session, TKey id)
		{
			AjaxResponse<object> response = await ProcessDeleteAsync<object, object>(session, $"delete?id={id}", null);
			return response.Success
					? new AjaxResponse(response.Success)
					: new AjaxResponse(response.Error, response.UnAuthorizedRequest)
				;
		}

		public async Task<AjaxResponse<TEntityDto>> UndeleteAsync(IAbpApiSession session, TKey id)
		{
			return await ProcessPostAsync<object, TEntityDto>(session, $"undelete?id={id}", null);
		}
	}
}
