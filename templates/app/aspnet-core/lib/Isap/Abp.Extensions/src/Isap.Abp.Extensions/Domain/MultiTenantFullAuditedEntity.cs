using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Isap.Abp.Extensions.Domain
{
	public interface IMultiTenantFullAuditedEntity<out TKey>: IMultiTenantEntity<TKey>, IFullAuditedObject
	{
	}

	public abstract class MultiTenantFullAuditedEntity<TKey>: FullAuditedEntity<TKey>, ICommonMultiTenant<Guid?>, IMultiTenantFullAuditedEntity<TKey>
	{
		protected MultiTenantFullAuditedEntity()
		{
		}

		protected MultiTenantFullAuditedEntity(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }

		object ICommonEntity.GetId() => Id;
	}

	public abstract class MultiTenantFullAuditedEntity<TKey, TIntf>: MultiTenantFullAuditedEntity<TKey>, IAssignable<TKey, TIntf>
		where TIntf: IMultiTenantEntity<TKey>
	{
		protected MultiTenantFullAuditedEntity()
		{
		}

		protected MultiTenantFullAuditedEntity(TKey id)
			: base(id)
		{
		}

		public virtual void Assign(TIntf source)
		{
			TenantId = source.TenantId;

			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}
}
