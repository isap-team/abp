using System;
using System.ComponentModel.DataAnnotations;

namespace Isap.CommonCore.Services
{
	public abstract class DocumentEntityDto<TKey>: SoftDeleteEntityDto<TKey>, IDocumentHeader, ICommonOwnedEntity<Guid?>
	{
		[MaxLength(DocumentEntityConsts.MaxDocNumberLength)]
		public string DocNumber { get; set; }

		public DateTime DocDate { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime LastModificationTime { get; set; }

		public Guid? OwnerId { get; set; }
	}

	public abstract class DocumentEntityDto<TKey, TDocumentState>: DocumentEntityDto<TKey>
	{
		public TDocumentState State { get; set; }
	}
}
