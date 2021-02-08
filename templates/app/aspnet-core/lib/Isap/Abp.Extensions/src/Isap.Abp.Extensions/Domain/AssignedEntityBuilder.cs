using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Domain
{
	public class AssignedEntityBuilder<TIntf, TImpl, TKey>: IEntityBuilder<TIntf, TImpl>
		where TImpl: IAssignable<TKey, TIntf>, new()
		where TIntf: ICommonEntity<TKey>
	{
		public TImpl CreateNew(TIntf source)
		{
			var result = new TImpl();
			if (source != null)
				result.Assign(source);
			return result;
		}
	}
}
