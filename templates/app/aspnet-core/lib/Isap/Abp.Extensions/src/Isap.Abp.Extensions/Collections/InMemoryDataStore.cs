using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Collections
{
	public class InMemoryDataStore<TKey, TEntity>
		: InMemoryDataCollection<TKey, TEntity>, IInMemoryDataStoreBuilder<TKey, TEntity>, IReferenceDataStore<TEntity, TKey>
		where TEntity: class, ICommonEntity<TKey>
	{
		public IDataFilterProvider DataFilterProvider { get; set; }
		public IPredicateBuilder PredicateBuilder { get; set; }

		public Task<TEntity> Get(TKey id)
		{
			return Task.FromResult(GetById(id));
		}

		public Task<List<TEntity>> GetMany(params TKey[] idList)
		{
			return Task.FromResult(idList.Select(GetById).ToList());
		}

		public async Task<ResultSet<TEntity>> GetPage(int pageNumber, int pageSize, ICollection<DataFilterValue> filterValues = null,
			ICollection<SortOption> sortOptions = null, bool countTotal = false)
		{
			Expression<Func<TEntity, bool>> predicate = await DataFilterProvider.ToExpressionAsync<TEntity>(filterValues);
			IEnumerable<TEntity> entities = GetAll()
					.Where(predicate.Compile())
				;
			if (sortOptions?.Count > 0)
			{
				IEnumerableSortProvider<TEntity> sortProvider = new SortOptionsSortProvider<TEntity>(PredicateBuilder, sortOptions);
				entities = sortProvider.Apply(entities);
			}

			return entities.ToResultSet(pageNumber, pageSize, countTotal ? GetCount() : (int?) null);
		}
	}
}
