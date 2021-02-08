using System;
using System.Runtime.InteropServices;
using Isap.Abp.Extensions.Metadata;
using Isap.CommonCore.Metadata;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterDefinition: IDataFilterDef
	{
		string Title { get; }
		Guid TargetEntityId { get; }
		int? DisplayOrder { get; }
		bool IsHidden { get; }
	}

	[Guid("A4436767-4CBA-465F-A18F-858C9CD7C4C2")]
	[EntityDef("DataFilter")]
	public class DataFilterDefinition: CommonEntityBase<Guid, IDataFilterDefinition>, IDataFilterDefinition
	{
		public string Title { get; set; }
		public Guid TargetEntityId { get; set; }
		public EntityDefinition TargetEntity { get; set; }
		public string TargetEntityType { get; set; }
		public DataFilterType Type { get; set; }
		public int? DisplayOrder { get; set; }
		public bool IsDisabled { get; set; }
		public bool IsHidden { get; set; }
		public string Options { get; set; }

		protected override void InternalAssign(IDataFilterDefinition source)
		{
			Title = source.Title;
			TargetEntityId = source.TargetEntityId;
			TargetEntityType = source.TargetEntityType;
			Type = source.Type;
			DisplayOrder = source.DisplayOrder;
			IsDisabled = source.IsDisabled;
			IsHidden = source.IsHidden;
			Options = source.Options;
		}

		public static IDataFilterDefinitionBuilder CreateBuilder(Guid dataFilterId, DataFilterType dataFilterType)
		{
			return DataFilterDefinitionBuilder.Create(dataFilterId, dataFilterType);
		}

		public static IDataFilterDefinitionBuilder CreateBuilder(string dataFilterId, DataFilterType dataFilterType)
		{
			return DataFilterDefinitionBuilder.Create(dataFilterId, dataFilterType);
		}
	}
}
