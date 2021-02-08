using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data
{
	public interface IAssignable<in TIntf>
		where TIntf: ICommonEntity
	{
		void Assign(TIntf source);
	}

	public interface IAssignable<TPrimaryKey, in TIntf>: IAssignable<TIntf>
		where TIntf: ICommonEntity<TPrimaryKey>
	{
	}
}
