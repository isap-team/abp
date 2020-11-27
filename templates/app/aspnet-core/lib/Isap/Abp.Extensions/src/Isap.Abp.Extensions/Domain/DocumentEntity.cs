using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Domain
{
	public interface IDocumentEntity: ICommonEntity, IDocumentHeader, ICommonOwnedEntity<Guid?>
	{
		DateTime CreationTime { get; }
		DateTime? LastModificationTime { get; }
	}

	public interface IDocumentEntity<out TKey>: ICommonEntity<TKey>, IDocumentEntity
	{
	}

	public abstract class DocumentEntity<TKey>: MultiTenantAggregateRoot<TKey>, IDocumentEntity<TKey>
	{
		protected DocumentEntity()
		{
		}

		protected DocumentEntity(TKey id)
			: base(id)
		{
		}

		public string DocNumber { get; set; }
		public DateTime DocDate { get; set; }

		public Guid? OwnerId { get; set; }
	}

	public abstract class DocumentEntity<TKey, TIntf>: DocumentEntity<TKey>, IAssignable<TKey, TIntf>
		where TIntf: IDocumentEntity<TKey>
	{
		protected DocumentEntity()
		{
		}

		protected DocumentEntity(TKey id)
			: base(id)
		{
		}

		public virtual void Assign(TIntf source)
		{
			DocNumber = source.DocNumber;
			DocDate = source.DocDate;
			OwnerId ??= source.OwnerId;

			InternalAssign(source);
		}

		protected abstract void InternalAssign(TIntf source);
	}

	public interface IDocumentEntity<out TKey, TDocumentState>: IDocumentEntity<TKey>
		where TDocumentState: struct
	{
		TDocumentState State { get; set; }
	}

	public abstract class DocumentEntity<TKey, TDocumentState, TIntf>: DocumentEntity<TKey, TIntf>, IDocumentEntity<TKey, TDocumentState>
		where TIntf: IDocumentEntity<TKey, TDocumentState>
		where TDocumentState: struct
	{
		protected DocumentEntity()
		{
		}

		protected DocumentEntity(TKey id)
			: base(id)
		{
		}

		public TDocumentState State { get; set; }

		public override void Assign(TIntf source)
		{
			State = source.State;

			base.Assign(source);
		}
	}
}
