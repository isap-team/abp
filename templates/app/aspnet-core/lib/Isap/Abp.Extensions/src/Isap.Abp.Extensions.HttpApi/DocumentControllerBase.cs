using System.Threading.Tasks;
using Isap.Abp.Extensions.Localization;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isap.Abp.Extensions
{
	public abstract class DocumentControllerBase<TEntityDto, TCreateDraftInput, TKey, TService, TLocalizationResource>
		: BasicControllerBase<TEntityDto, TKey, TService, TLocalizationResource>, IDocumentAppService<TEntityDto, TCreateDraftInput, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IDocumentAppService<TEntityDto, TCreateDraftInput, TKey>
		where TLocalizationResource: ILocalizationResource
	{
		[HttpPost]
		[Route("get-or-create-draft")]
		public Task<TEntityDto> GetOrCreateDraft(TCreateDraftInput input)
		{
			return AppService.GetOrCreateDraft(input);
		}

		[HttpGet]
		[Route("get-draft")]
		public Task<TEntityDto> GetDraft(TKey id)
		{
			return AppService.GetDraft(id);
		}

		[HttpPut]
		[Route("save-draft")]
		public Task<TEntityDto> SaveDraft(TEntityDto entry)
		{
			return AppService.SaveDraft(entry);
		}

		[HttpDelete]
		[Route("delete-draft")]
		public Task DeleteDraft(TKey id)
		{
			return AppService.DeleteDraft(id);
		}

		[HttpPost]
		[Route("delete-draft")]
		public Task DeleteDraft(TEntityDto entry)
		{
			return AppService.DeleteDraft(entry);
		}
	}

	public abstract class DocumentControllerBase<TEntityDto, TCreateDraftInput, TKey, TService>
		: DocumentControllerBase<TEntityDto, TCreateDraftInput, TKey, TService, AbpExtensionsResource>
		where TEntityDto: ICommonEntityDto<TKey>
		where TService: IDocumentAppService<TEntityDto, TCreateDraftInput, TKey>
	{
	}
}
