using Isap.CommonCore.Services;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions.Domain
{
	public interface ISoftDeleteEntity<out TKey>: ICommonEntity<TKey>, ISoftDelete
	{
	}

	public class SoftDeleteEntity<TKey>: Entity<TKey>, ISoftDeleteEntity<TKey>
	{
		public SoftDeleteEntity()
		{
		}

		public SoftDeleteEntity(TKey id)
			: base(id)
		{
		}

		public bool IsDeleted { get; set; }

		object ICommonEntity.GetId() => Id;
	}
}
