using System.Threading.Tasks;
using Isap.Abp.Extensions.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class DocumentDomainManagerBase<TIntf, TImpl, TKey, TDataRepository, TCreateDraftInput>
		: DomainManagerBase<TIntf, TImpl, TKey, TDataRepository>, IDocumentDomainManager<TIntf, TImpl, TKey, TCreateDraftInput>
		where TIntf: class, IDocumentEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf, IAssignable<TKey, TIntf>, new()
		where TDataRepository: class, IRepository<TImpl, TKey>
	{
		public abstract bool IsDraft(TIntf entry);

		public abstract Task<TIntf> GetOrCreateDraft(TCreateDraftInput input);
	}
}
