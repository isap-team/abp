using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Collections
{
	public class InMemoryDataStore<TKey, TEntity>
		: InMemoryDataCollection<TKey, TEntity>, IInMemoryDataStoreBuilder<TKey, TEntity>, IReferenceDataStore<TEntity, TKey>
		where TEntity: class, ICommonEntity<TKey>
	{
		public IPredicateBuilder PredicateBuilder { get; set; }
		public IDataFilterProvider DataFilterProvider { get; set; }

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

	public class InMemoryDataStore<TIntf, TImpl, TKey>
		: InMemoryDataStore<TKey, TIntf>, IReferenceDataStore<TIntf, TImpl, TKey>, IEnumerableSortProvider<TImpl>
		where TIntf: class, ICommonEntity<TKey> where TImpl: class, TIntf
	{
		//public IIsapDbContextProvider DbContextProvider => throw new NotSupportedException();
		public IDataFilter DataFilter { get; set; }

		public async Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, Expression<Func<TImpl, bool>> predicate, ICollection<SortOption> sortOptions = null, bool countTotal = false)
		{
			IEnumerable<TIntf> entities = await GetAll(predicate, sortOptions);
			return entities.ToResultSet(pageNumber, pageSize, countTotal ? GetCount() : (int?) null);
		}

		public Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, Expression<Func<TImpl, bool>> predicate, IOrderSpecification<TImpl> orderSpecification, bool countTotal = false)
		{
			throw new NotImplementedException();
		}

		public async Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, IFilterSpecification<TImpl> filterSpecification = null, IOrderSpecification<TImpl> orderSpecification = null,
			bool countTotal = false)
		{
			return await GetPageInternal(pageNumber, pageSize, countTotal, filterSpecification, orderSpecification);
		}

		public Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, List<SpecificationParameters> specifications, bool countTotal = false)
		{
			throw new NotImplementedException();
		}

		public async Task<List<TIntf>> GetAll(Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null, bool asNoTracking = false)
		{
			predicate = predicate ?? PredicateBuilder.True<TImpl>();
			IEnumerable<TIntf> entities = base.GetAll()
					.Cast<TImpl>()
					.Where(predicate.Compile())
				;

			await Task.Yield();

			if (sortOptions?.Count > 0)
			{
				IEnumerableSortProvider<TIntf> sortProvider = new SortOptionsSortProvider<TIntf>(PredicateBuilder, sortOptions);
				entities = sortProvider.Apply(entities);
			}

			return entities.ToList();
		}

		public Task<int> CountAsync(Expression<Func<TImpl, bool>> predicate = null)
		{
			predicate = predicate ?? PredicateBuilder.True<TImpl>();
			IEnumerable<TIntf> entities = base.GetAll()
					.Cast<TImpl>()
					.Where(predicate.Compile())
				;
			return Task.FromResult(entities.Count());
		}

		public async Task<TIntf> GetFirst(Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null)
		{
			IEnumerable<TIntf> entities = await GetAll(predicate, sortOptions);
			return entities.FirstOrDefault();
		}

		public async Task<TIntf> GetSingle(Expression<Func<TImpl, bool>> predicate)
		{
			IEnumerable<TIntf> entities = await GetAll(predicate);
			return entities.SingleOrDefault();
		}

		public async Task<TImpl> GetEditable(TKey id)
		{
			return (TImpl) await Get(id);
		}

		public async Task<TImpl> GetEditable(TIntf entry)
		{
			if (entry is TImpl e)
				return e;
			return await GetEditable(entry.Id);
		}

		public TImpl AsEditable(TIntf entry)
		{
			return entry as TImpl ?? throw new InvalidOperationException();
		}

		public Task<TIntf> LoadRelatedData(TIntf entry)
		{
			throw new NotSupportedException();
		}

		protected virtual async Task<ResultSet<TIntf>> GetPageInternal(int pageNumber, int pageSize, bool countTotal = false,
			IFilterSpecification<TImpl> filterSpecification = null, IOrderSpecification<TImpl> orderSpecification = null)
		{
			if (pageNumber <= 0)
				throw new ArgumentException("Page number should be greater then zero.", nameof(pageNumber));
			if (pageSize <= 0)
				throw new ArgumentException("Page size should be greater then zero.", nameof(pageSize));

			filterSpecification = filterSpecification ?? new TrueSpecification<TImpl>();
			orderSpecification = orderSpecification ?? new NoOrderBySpecification<TImpl>();

			bool isIgnoreSafeDeleteFilter = filterSpecification.IsIgnoreSafeDeleteFilter;
			bool isIgnoreMultiTenantFilter = filterSpecification.IsIgnoreMultiTenantFilter;

			using (isIgnoreSafeDeleteFilter ? DataFilter.Disable<ISoftDelete>() : DataFilter.Enable<ISoftDelete>())
			using (isIgnoreMultiTenantFilter ? DataFilter.Disable<IMultiTenant>() : DataFilter.Enable<IMultiTenant>())
			{
				Expression<Func<TImpl, bool>> predicate = filterSpecification.IsSatisfiedBy();

				List<TIntf> allSatisfied = await GetAll(predicate);

				int? totalCount = countTotal ? allSatisfied.Count : (int?) null;
				if (totalCount.HasValue)
					if (totalCount.Value <= (pageNumber - 1) * pageSize)
						return Enumerable.Empty<TIntf>().ToResultSet(pageNumber, pageSize, totalCount);
				IEnumerableSortProvider<TImpl> sortProvider = orderSpecification.IsEmpty
						? (IEnumerableSortProvider<TImpl>) this
						: orderSpecification
					;
				return allSatisfied
						.Cast<TImpl>()
						.OrderBy(sortProvider)
						.Cast<TIntf>()
						.ToResultSet(pageNumber, pageSize, totalCount)
					;
			}
		}

		/// <summary>
		///     Включает сортировку выбираемых данных. По-умолчанию сортировка по идентификатору. Сортировку можно переопределить в
		///     наследниках.
		/// </summary>
		/// <param name="query">Объект, необходимый для построения запроса выборки данных из БД.</param>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected virtual IEnumerable<TImpl> DefaultSortQuery(IEnumerable<TImpl> query)
		{
			return query
					.OrderBy(e => ((IEntity<TKey>) e).Id)
				;
		}

		IEnumerable<TImpl> IEnumerableSortProvider<TImpl>.Apply(IEnumerable<TImpl> query)
		{
			return DefaultSortQuery(query);
		}

		IEnumerable IEnumerableSortProvider.Apply(IEnumerable query)
		{
			return DefaultSortQuery(query.Cast<TImpl>());
		}
	}
}
