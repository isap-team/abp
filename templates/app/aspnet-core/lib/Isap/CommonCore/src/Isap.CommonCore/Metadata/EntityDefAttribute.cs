using System;

namespace Isap.CommonCore.Metadata
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class EntityDefAttribute: Attribute
	{
		public EntityDefAttribute(string entityId, string entityName = null)
		{
			EntityId = new Guid(entityId);
			EntityName = entityName;
		}

		public Guid EntityId { get; }
		public string EntityName { get; }
	}
}
