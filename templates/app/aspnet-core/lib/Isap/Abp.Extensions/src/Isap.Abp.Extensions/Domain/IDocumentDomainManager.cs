using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Domain
{
	public interface IDocumentDomainManager<TIntf, TImpl, in TKey, in TCreateDraftInput>: IDomainManager<TIntf, TImpl, TKey>
		where TIntf: class, IDocumentEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
	{
		bool IsDraft(TIntf entry);
		Task<TIntf> GetOrCreateDraft(TCreateDraftInput input);
	}
}
