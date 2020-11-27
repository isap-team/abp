using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using JetBrains.Annotations;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Domain
{
	public interface IMultiTenantEntity<out TKey>: ISoftDeleteEntity<TKey>, IMultiTenant
	{
	}

	public abstract class MultiTenantEntity<TKey>: SoftDeleteEntity<TKey>, ICommonMultiTenant<Guid?>, IMultiTenantEntity<TKey>
	{
		protected MultiTenantEntity()
		{
		}

		protected MultiTenantEntity(TKey id)
			: base(id)
		{
		}

		public Guid? TenantId { get; set; }
	}

	public abstract class MultiTenantEntity<TKey, TIntf>: MultiTenantEntity<TKey>, IAssignable<TKey, TIntf>
		where TIntf: IMultiTenantEntity<TKey>
	{
		protected MultiTenantEntity()
		{
		}

		protected MultiTenantEntity(TKey id)
			: base(id)
		{
		}

		public virtual void Assign([NotNull] TIntf source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			TenantId = source.TenantId;

			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}
}
