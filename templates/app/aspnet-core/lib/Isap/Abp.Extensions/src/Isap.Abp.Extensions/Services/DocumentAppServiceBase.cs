using System;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.CommonCore.Services;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Services
{
	public abstract class DocumentAppServiceBase<TEntityDto, TCreateDraftInput, TIntf, TImpl, TKey, TDomainManager>
		: BasicAppServiceBase<TEntityDto, TIntf, TImpl, TKey, TDomainManager>, IDocumentAppService<TEntityDto, TCreateDraftInput, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
	{
		public virtual Task<TEntityDto> GetOrCreateDraft(TCreateDraftInput input)
		{
			throw new NotImplementedException();
		}

		public virtual Task<TEntityDto> GetDraft(TKey id)
		{
			throw new NotImplementedException();
		}

		public virtual Task<TEntityDto> SaveDraft(TEntityDto entry)
		{
			throw new NotImplementedException();
		}

		public virtual Task DeleteDraft(TKey id)
		{
			throw new NotImplementedException();
		}

		public virtual Task DeleteDraft(TEntityDto entry)
		{
			throw new NotImplementedException();
		}
	}
}
