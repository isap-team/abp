using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Isap.Abp.Extensions.DataFilters;
using Isap.CommonCore.Metadata;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Metadata
{
	/// <summary>
	///     Интерфейс к и информации о метаданных сущности.
	/// </summary>
	public interface IEntityDefinition: ICommonEntity<Guid>
	{
		/// <summary>
		///     Внутреннее наименование сущности.
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Полное имя класса сущности.
		/// </summary>
		string EntityTypeName { get; }

		/// <summary>
		///     Тип сущности.
		/// </summary>
		Type EntityType { get; }

		/// <summary>
		///     Настройки для отображения списка в JSON-формате.
		/// </summary>
		EntityListOptions ListOptions { get; }

		/// <summary>
		///     Настройки для отображения ввиде справочника в JSON-формате.
		/// </summary>
		EntityReferenceOptions ReferenceOptions { get; }

		ICollection<IDataFilterDefinition> DataFilters { get; }
	}

	[Guid("8FDBD3F6-4D05-411C-BAE2-E286367E2F58")]
	[EntityDef("Entity")]
	public class EntityDefinition: CommonEntityBase<Guid, IEntityDefinition>, IEntityDefinition
	{
		public string Name { get; set; }
		public string EntityTypeName { get; set; }
		public EntityListOptions ListOptions { get; set; }
		public EntityReferenceOptions ReferenceOptions { get; set; }

		public List<DataFilterDefinition> DataFilters { get; set; }
		ICollection<IDataFilterDefinition> IEntityDefinition.DataFilters => DataFilters?.Cast<IDataFilterDefinition>().ToList();

		Type IEntityDefinition.EntityType => Type.GetType(EntityTypeName);

		protected override void InternalAssign(IEntityDefinition source)
		{
			Name = source.Name;
			EntityTypeName = source.EntityTypeName;
			ListOptions = source.ListOptions;
			ReferenceOptions = source.ReferenceOptions;
		}

		public static IEntityDefinitionBuilder CreateBuilder(Type entityType)
		{
			return EntityDefinitionBuilder.Create(entityType);
		}

		public static IEntityDefinitionBuilder CreateBuilder<TEntity>()
		{
			return EntityDefinitionBuilder.Create(typeof(TEntity));
		}
	}
}
