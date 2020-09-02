using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Isap.CommonCore.Services.InMemoryServices
{
	public abstract class InMemoryReferenceAppServiceBase<TEntityDto, TKey>: IReferenceAppService<TEntityDto, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
	{
		protected InMemoryReferenceAppServiceBase(
			IInMemoryEntityCollectionProvider collectionProvider)
		{
			Entries = collectionProvider.GetCollection<TKey, TEntityDto>();
		}

		protected InMemoryReferenceAppServiceBase()
			: this(InMemoryEntityCollectionProvider.Default)
		{
		}

		protected ConcurrentDictionary<TKey, TEntityDto> Entries { get; set; }

		public Task<TEntityDto> Get(TKey id)
		{
			TEntityDto entry = Entries.TryGetValue(id, out var result) ? result : null;
			return Task.FromResult(entry);
		}

		public Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			return Task.FromResult(idList.ToDictionary(id => id, TryGetEntry));
		}

		public Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			ResultSet<TEntityDto> resultSet = Entries
				.Select(pair => pair.Value)
				//.Skip((pageNumber - 1) * pageSize)
				//.Take(pageSize + 1)
				.ToResultSet(pageNumber, pageSize, countTotal ? Entries.Count : (int?) null);
			return Task.FromResult(resultSet);
		}

		public Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, List<SpecificationParameters> specifications = null)
		{
			ResultSet<TEntityDto> resultSet = Entries
				.Select(pair => pair.Value)
				//.Skip((pageNumber - 1) * pageSize)
				//.Take(pageSize + 1)
				.ToResultSet(pageNumber, pageSize, countTotal ? Entries.Count : (int?) null);
			return Task.FromResult(resultSet);
		}

#pragma warning disable 1066
		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.GetPage(int pageNumber, int pageSize, bool countTotal = false,
			ICollection<DataFilterValueDto> filterValues = null)
#pragma warning restore 1066
		{
			return GetPage(pageNumber, pageSize, countTotal, new QueryOptionsDto(filterValues));
		}

#pragma warning disable 1066
		Task<ResultSet<TEntityDto>> IReferenceAppService<TEntityDto, TKey>.QueryPage(int pageNumber, int pageSize, bool countTotal = false,
			QueryOptionsDto queryOptions = null)
#pragma warning restore 1066
		{
			return GetPage(pageNumber, pageSize, countTotal, queryOptions);
		}

		protected TEntityDto TryGetEntry(TKey id)
		{
			return Entries.TryGetValue(id, out var result) ? result : null;
		}
	}

	public abstract class InMemoryReferenceAppServiceBase<TEntityDto>: InMemoryReferenceAppServiceBase<TEntityDto, Guid>
		where TEntityDto: class, ICommonEntityDto<Guid>
	{
	}
}
