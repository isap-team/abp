using System;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Extensions;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class DataStoreBase: DomainServiceBase
	{
		protected IIsapDbContextProviderResolver DbContextProviderResolver => LazyGetRequiredService<IIsapDbContextProviderResolver>();

		protected static string ReadEmbeddedResourceAsString(Type namespaceType, string name)
		{
			return namespaceType.Assembly.ReadEmbeddedResourceAsString(namespaceType, name);
		}

		protected static string ReadEmbeddedQuery(Type namespaceType, string name)
		{
			return ReadEmbeddedResourceAsString(namespaceType, name).Replace("{", "{{").Replace("}", "}}");
		}
	}
}
