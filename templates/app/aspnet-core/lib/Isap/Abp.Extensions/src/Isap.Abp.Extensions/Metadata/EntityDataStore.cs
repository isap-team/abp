using System;
using Isap.Abp.Extensions.Collections;
using Isap.Abp.Extensions.Domain;

namespace Isap.Abp.Extensions.Metadata
{
	public interface IEntityDataStore: IReferenceDataStore<IEntityDefinition, Guid>
	{
	}

	public interface IEntityDataStoreBuilder: IInMemoryDataStoreBuilder<Guid, IEntityDefinition>
	{
	}

	public class EntityDataStore: InMemoryDataStore<Guid, IEntityDefinition>, IEntityDataStoreBuilder, IEntityDataStore
	{
	}
}
