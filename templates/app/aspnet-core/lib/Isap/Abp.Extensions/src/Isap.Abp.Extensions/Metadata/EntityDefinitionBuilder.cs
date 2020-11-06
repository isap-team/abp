using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using Isap.CommonCore.Metadata;

namespace Isap.Abp.Extensions.Metadata
{
	public interface IEntityDefinitionBuilder
	{
		IEntityDefinitionBuilder Register(Action<EntityDefinition> buildAction);
		IEntityDefinition Build();
		IEntityDefinition BuildAndRegister(bool replace = true);
	}

	public class EntityDefinitionBuilder: IEntityDefinitionBuilder
	{
		private static readonly Regex _intfNameStartsWithIRegex = new Regex("^I[A-Z].*$", RegexOptions.Compiled | RegexOptions.Singleline);

		private readonly List<Action<EntityDefinition>> _buildActions = new List<Action<EntityDefinition>>();
		private readonly Guid _entityId;
		private readonly string _entityName;
		private readonly Type _entityType;

		private EntityDefinitionBuilder(Guid entityId, string entityName, Type entityType)
		{
			_entityId = entityId;
			_entityName = entityName;
			_entityType = entityType;
		}

		protected IMetadataRegistrar MetadataRegistrar => MetadataProvider.Instance;

		public IEntityDefinitionBuilder Register(Action<EntityDefinition> buildAction)
		{
			_buildActions.Add(buildAction);
			return this;
		}

		public IEntityDefinition Build()
		{
			var entityDef = new EntityDefinition
				{
					Id = _entityId,
					EntityTypeName = _entityType.AssemblyQualifiedName,
					Name = _entityName,
				};
			foreach (Action<EntityDefinition> buildAction in _buildActions)
				buildAction(entityDef);
			return entityDef;
		}

		public IEntityDefinition BuildAndRegister(bool replace = true)
		{
			var entityDef = Build();
			return MetadataRegistrar.Register(entityDef, replace);
		}

		public static IEntityDefinitionBuilder Create(Type entityType)
		{
			EntityDefAttribute attr = entityType.GetAttribute<EntityDefAttribute>();
			return attr == null
					? new EntityDefinitionBuilder(entityType.GUID, NormalizeEntityName(entityType), entityType)
					: new EntityDefinitionBuilder(entityType.GUID, attr.EntityName ?? NormalizeEntityName(entityType), entityType)
				;
		}

		private static string NormalizeEntityName(Type entityType)
		{
			string name = entityType.Name.RemovePostFix("Base");

			if (entityType.IsInterface && _intfNameStartsWithIRegex.IsMatch(name))
				name = name.Substring(1);

			return name;
		}
	}
}
