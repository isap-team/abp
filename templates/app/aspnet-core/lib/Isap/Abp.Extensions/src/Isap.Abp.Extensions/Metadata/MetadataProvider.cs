using System;
using System.Collections.Concurrent;

namespace Isap.Abp.Extensions.Metadata
{
	public interface IMetadataProvider
	{
		IEntityDefinition TryGetEntityDef(Guid entityId);
		IEntityDefinition GetEntityDef(Guid entityId);
	}

	public interface IMetadataRegistrar
	{
		IEntityDefinition Register(IEntityDefinition entityDef, bool replace = true);
	}

	public class MetadataProvider: IMetadataProvider, IMetadataRegistrar
	{
		public static MetadataProvider Instance = new MetadataProvider();

		private readonly ConcurrentDictionary<Guid, IEntityDefinition> _entityDefMap = new ConcurrentDictionary<Guid, IEntityDefinition>();

		private MetadataProvider()
		{
		}

		public IEntityDefinition TryGetEntityDef(Guid entityId)
		{
			return _entityDefMap.TryGetValue(entityId, out IEntityDefinition entityDef) ? entityDef : null;
		}

		public IEntityDefinition GetEntityDef(Guid entityId)
		{
			return TryGetEntityDef(entityId) ?? throw new InvalidOperationException($"Can't find entity definition with id = '{entityId}'.");
		}

		public IEntityDefinition Register(IEntityDefinition entityDef, bool replace = true)
		{
			if (entityDef.Id.Equals(Guid.Empty)) return entityDef;

			if (replace)
				_entityDefMap.AddOrUpdate(entityDef.Id, entityDef, (id, def) => entityDef);
			else
				if (!_entityDefMap.TryAdd(entityDef.Id, entityDef))
					throw new InvalidOperationException($"Can't register entity definition with id = '{entityDef.Id}' and entity type = '{entityDef.EntityTypeName}'. An entity definition with same id is registered already.");
			return entityDef;
		}
	}
}
