using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;
using Volo.Abp.Domain.Entities;

namespace Isap.Abp.Extensions
{
	public abstract class CommonEntityBase<TPrimaryKey, TIntf>: ICommonEntity<TPrimaryKey>, IAssignable<TPrimaryKey, TIntf>, IEntity<TPrimaryKey>
		where TIntf: ICommonEntity<TPrimaryKey>
	{
		public TPrimaryKey Id { get; set; }

		public void Assign(TIntf source)
		{
			InternalAssign(source);
		}

		object ICommonEntity.GetId() => Id;

		object[] IEntity.GetKeys() => new object[] { Id };

		protected abstract void InternalAssign(TIntf source);
	}
}
