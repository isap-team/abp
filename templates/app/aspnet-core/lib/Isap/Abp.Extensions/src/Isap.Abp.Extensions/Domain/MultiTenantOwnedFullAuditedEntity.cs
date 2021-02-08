using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Isap.Abp.Extensions.Domain
{
	public interface IMultiTenantOwnedFullAuditedEntity<out TKey>: IMultiTenantEntity<TKey>, ICommonOwnedEntity<Guid?>, IFullAuditedObject
	{
	}

	public abstract class MultiTenantOwnedFullAuditedEntity<TKey>: FullAuditedEntity<TKey>, ICommonMultiTenant<Guid?>, IMultiTenantOwnedFullAuditedEntity<TKey>
	{
		protected MultiTenantOwnedFullAuditedEntity()
		{
		}

		protected MultiTenantOwnedFullAuditedEntity(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }
		public Guid? OwnerId { get; set; }

		object ICommonEntity.GetId() => Id;
	}

	public abstract class MultiTenantOwnedFullAuditedEntity<TKey, TIntf>: MultiTenantOwnedFullAuditedEntity<TKey>, IAssignable<TKey, TIntf>
		where TIntf: IMultiTenantEntity<TKey>, ICommonOwnedEntity<Guid?>
	{
		protected MultiTenantOwnedFullAuditedEntity()
		{
		}

		protected MultiTenantOwnedFullAuditedEntity(TKey id)
			: base(id)
		{
		}

		public virtual void Assign(TIntf source)
		{
			TenantId = source.TenantId;
			OwnerId = source.OwnerId;

			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}
}
