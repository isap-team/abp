using System.Threading.Tasks;
using Abp.Web.Models;
using Isap.Abp.Extensions.Api.Clients;

namespace Isap.Abp.Extensions.Api
{
	public interface IDocumentApiManager<TEntityDto, TKey, in TCreateDraftInput>: IBasicApiManager<TEntityDto, TKey>
	{
		Task<AjaxResponse<TEntityDto>> GetOrCreateDraft(IAbpApiSession session, TCreateDraftInput input);
		Task<AjaxResponse<TEntityDto>> GetDraft(IAbpApiSession session, TKey id);
		Task<AjaxResponse<TEntityDto>> SaveDraft(IAbpApiSession session, TEntityDto entry);
		Task<AjaxResponse> DeleteDraft(IAbpApiSession session, TKey id);
		Task<AjaxResponse> DeleteDraft(IAbpApiSession session, TEntityDto entry);
	}

	public abstract class DocumentApiManager<TEntityDto, TKey, TApiClient, TCreateDraftInput>
		: BasicApiManager<TEntityDto, TKey, TApiClient>, IDocumentApiManager<TEntityDto, TKey, TCreateDraftInput>
		where TApiClient: IAbpApiClient
	{
		protected DocumentApiManager(TApiClient client, string serviceBaseUrl)
			: base(client, serviceBaseUrl)
		{
		}

		public async Task<AjaxResponse<TEntityDto>> GetOrCreateDraft(IAbpApiSession session, TCreateDraftInput input)
		{
			return await ProcessPostAsync<TCreateDraftInput, TEntityDto>(session, "getOrCreateDraft", input);
		}

		public async Task<AjaxResponse<TEntityDto>> GetDraft(IAbpApiSession session, TKey id)
		{
			return await ProcessGetAsync<object, TEntityDto>(session, $"getDraft?id={id}", null);
		}

		public async Task<AjaxResponse<TEntityDto>> SaveDraft(IAbpApiSession session, TEntityDto entry)
		{
			return await ProcessPostAsync<TEntityDto, TEntityDto>(session, "saveDraft", entry);
		}

		public async Task<AjaxResponse> DeleteDraft(IAbpApiSession session, TKey id)
		{
			AjaxResponse<object> response = await ProcessDeleteAsync<object, object>(session, $"deleteDraft?id={id}", null);
			return response.Success
					? new AjaxResponse(response.Success)
					: new AjaxResponse(response.Error, response.UnAuthorizedRequest)
				;
		}

		public async Task<AjaxResponse> DeleteDraft(IAbpApiSession session, TEntityDto entry)
		{
			AjaxResponse<object> response = await ProcessPostAsync<TEntityDto, object>(session, "deleteDraftForEntry", entry);
			return response.Success
					? new AjaxResponse(response.Success)
					: new AjaxResponse(response.Error, response.UnAuthorizedRequest)
				;
		}
	}
}
