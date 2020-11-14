using System;

namespace Isap.CommonCore.Metadata
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class EntityDefAttribute: Attribute
	{
		public EntityDefAttribute(string entityName = null)
		{
			EntityName = entityName;
		}

		public string EntityName { get; }
	}
}
