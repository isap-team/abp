using System;

namespace Isap.CommonCore.API
{
	public class ApiCollectionJoinAttribute: Attribute
	{
		public ApiCollectionJoinAttribute(string delimiter)
		{
			Delimiter = delimiter;
		}

		public string Delimiter { get; }
	}
}
