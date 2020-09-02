using System.Collections.Generic;
using Isap.Abp.Extensions.DataFilters;

namespace Isap.Abp.Extensions.Metadata
{
	public static class MetadataBuilderExtensions
	{
		#region IEntityDefinitionBuilder's extensions

		public static IEntityDefinitionBuilder WithName(this IEntityDefinitionBuilder builder, string entityName)
			=> builder.Register(def => def.Name = entityName);

		public static IEntityDefinitionBuilder WithDataFilter(this IEntityDefinitionBuilder builder, IDataFilterDefinitionBuilder dataFilterBuilder)
			=> builder.Register(def =>
				{
					if (def.DataFilters == null) def.DataFilters = new List<DataFilterDefinition>();
					def.DataFilters.Add(
						dataFilterBuilder
							.WithTargetEntity(def)
							.WithDisplayOrderIfEmpty((def.DataFilters.Count + 1) * 10)
							.Build()
					);
				});

		#endregion

		#region IDataFilterDefinitionBuilder's extensions

		public static IDataFilterDefinitionBuilder WithTargetEntity(this IDataFilterDefinitionBuilder builder, EntityDefinition targetEntity)
			=> builder.Register(def =>
				{
					def.TargetEntity = targetEntity;
					def.TargetEntityId = targetEntity.Id;
					def.TargetEntityType = targetEntity.EntityTypeName;
				});

		public static IDataFilterDefinitionBuilder WithTitle(this IDataFilterDefinitionBuilder builder, string title)
			=> builder.Register(def => def.Title = title);

		public static IDataFilterDefinitionBuilder WithOptions(this IDataFilterDefinitionBuilder builder, DataFilterOptions options)
			=> builder.Register(def => def.Options = options.Serialize());

		public static IDataFilterDefinitionBuilder WithDisplayOrder(this IDataFilterDefinitionBuilder builder, int displayOrder)
			=> builder.Register(def => def.DisplayOrder = displayOrder);

		private static IDataFilterDefinitionBuilder WithDisplayOrderIfEmpty(this IDataFilterDefinitionBuilder builder, int displayOrder)
			=> builder.Register(def => def.DisplayOrder = def.DisplayOrder ?? displayOrder);

		public static IDataFilterDefinitionBuilder IsDisabled(this IDataFilterDefinitionBuilder builder)
			=> builder.Register(def => def.IsDisabled = true);

		public static IDataFilterDefinitionBuilder IsHidden(this IDataFilterDefinitionBuilder builder)
			=> builder.Register(def => def.IsHidden = true);

		#endregion
	}
}
