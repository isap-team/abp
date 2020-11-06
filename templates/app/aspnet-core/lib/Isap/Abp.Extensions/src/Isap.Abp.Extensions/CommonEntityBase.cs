using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions
{
	public abstract class CommonEntityBase<TPrimaryKey, TIntf>: ICommonEntity<TPrimaryKey>, IAssignable<TPrimaryKey, TIntf>
		where TIntf: ICommonEntity<TPrimaryKey>
	{
		public TPrimaryKey Id { get; set; }

		public void Assign(TIntf source)
		{
			InternalAssign(source);
		}

		object ICommonEntity.GetId() => Id;

		protected abstract void InternalAssign(TIntf source);
	}
}
