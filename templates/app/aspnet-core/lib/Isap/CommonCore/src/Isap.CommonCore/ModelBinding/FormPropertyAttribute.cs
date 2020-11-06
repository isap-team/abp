using System;

namespace Isap.CommonCore.ModelBinding
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class FormPropertyAttribute: Attribute
	{
		public FormPropertyAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}

		public string PropertyName { get; }
	}
}
