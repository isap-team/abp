using System;
using Isap.Abp.Extensions.Collections;
using Isap.Abp.Extensions.Domain;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterDataStore: IReferenceDataStore<IDataFilterDefinition, Guid>
	{
	}

	public interface IDataFilterDataStoreBuilder: IInMemoryDataStoreBuilder<Guid, IDataFilterDefinition>
	{
	}

	public class DataFilterDataStore: InMemoryDataStore<Guid, IDataFilterDefinition>, IDataFilterDataStoreBuilder, IDataFilterDataStore
	{
	}
}
