using System.Threading.Tasks;

namespace Isap.CommonCore.Services
{
	public interface IDocumentAppService<TEntityDto, in TCreateDraftInput, TKey>: IBasicAppService<TEntityDto, TKey>
		where TEntityDto: ICommonEntityDto<TKey>
	{
		Task<TEntityDto> GetOrCreateDraft(TCreateDraftInput input);
		Task<TEntityDto> GetDraft(TKey id);
		Task<TEntityDto> SaveDraft(TEntityDto entry);
		Task DeleteDraft(TKey id);
		Task DeleteDraft(TEntityDto entry);
	}
}
