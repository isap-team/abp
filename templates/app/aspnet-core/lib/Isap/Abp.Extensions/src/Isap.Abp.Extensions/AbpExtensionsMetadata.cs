using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.DataFilters.Concrete;
using Isap.Abp.Extensions.Metadata;

namespace Isap.Abp.Extensions
{
	public static class AbpExtensionsMetadata
	{
		public static class EntityDefs
		{
			public static readonly IEntityDefinition EntityDef =
				EntityDefinition.CreateBuilder<EntityDefinition>()
					.Build();

			public static readonly IEntityDefinition DataFilterDef =
				EntityDefinition.CreateBuilder<DataFilterDefinition>()
					.WithDataFilter(DataFilterDefinition.CreateBuilder("F61D0439-BE71-45FD-A92A-473E4E8ECC18", DataFilterType.SelectOne)
						.WithTitle("Тип сущности")
						.WithOptions(new SelectOneDataFilterOptions(
							nameof(DataFilterDefinition.TargetEntityId),
							DataFilterInputTypes.DropDownSelect,
							entityId: EntityDef.Id))
					)
					.Build();
		}
	}
}
