using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Isap.CommonCore.Services.InMemoryServices
{
	public interface IInMemoryEntityCollectionProvider
	{
		ConcurrentDictionary<TKey, TEntityDto> GetCollection<TKey, TEntityDto>()
			where TEntityDto: class, ICommonEntityDto<TKey>;

		ConcurrentDictionary<Guid, TEntityDto> GetCollection<TEntityDto>()
			where TEntityDto: class, ICommonEntityDto<Guid>;
	}

	public class InMemoryEntityCollectionProvider: IInMemoryEntityCollectionProvider
	{
		public static InMemoryEntityCollectionProvider Default = new InMemoryEntityCollectionProvider(new ConcurrentDictionary<Type, IDictionary>());

		private readonly ConcurrentDictionary<Type, IDictionary> _collectionMap;

		public InMemoryEntityCollectionProvider(ConcurrentDictionary<Type, IDictionary> collectionMap)
		{
			_collectionMap = collectionMap;
		}

		public InMemoryEntityCollectionProvider()
			: this(Default._collectionMap)
		{
		}

		public ConcurrentDictionary<TKey, TEntityDto> GetCollection<TKey, TEntityDto>()
			where TEntityDto: class, ICommonEntityDto<TKey>
		{
			return (ConcurrentDictionary<TKey, TEntityDto>) _collectionMap.GetOrAdd(typeof(TEntityDto), _ => new ConcurrentDictionary<TKey, TEntityDto>());
		}

		public ConcurrentDictionary<Guid, TEntityDto> GetCollection<TEntityDto>()
			where TEntityDto: class, ICommonEntityDto<Guid>
		{
			return GetCollection<Guid, TEntityDto>();
		}
	}
}
