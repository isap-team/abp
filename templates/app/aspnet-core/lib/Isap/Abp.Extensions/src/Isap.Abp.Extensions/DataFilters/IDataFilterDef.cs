using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterDef: ICommonEntity<Guid>
	{
		DataFilterType Type { get; }
		string TargetEntityType { get; }
		bool IsDisabled { get; }
		string Options { get; }
	}
}
