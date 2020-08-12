using System;

namespace Isap.CommonCore.DependencyInjection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyInjectAttribute: Attribute
	{
		public PropertyInjectAttribute()
		{
			IsOptional = true;
		}

		public string DependencyKey { get; set; }
		public bool IsOptional { get; set; }
	}
}
