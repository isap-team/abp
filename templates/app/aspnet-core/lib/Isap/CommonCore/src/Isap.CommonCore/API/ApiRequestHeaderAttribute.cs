using System;

namespace Isap.CommonCore.API
{
	public sealed class ApiRequestHeaderAttribute: Attribute
	{
		public ApiRequestHeaderAttribute(string name, string format = null)
		{
			Name = name;
			Format = format ?? "{0}";
		}

		public string Name { get; }
		public string Format { get; }
	}
}
