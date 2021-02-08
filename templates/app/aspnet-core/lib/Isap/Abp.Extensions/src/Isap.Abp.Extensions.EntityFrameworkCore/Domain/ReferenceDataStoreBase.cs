using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data.Specifications;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.EntityFrameworkCore.Extensions;
using Isap.CommonCore.Services;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Isap.Abp.Extensions.Domain
{
	public abstract class ReferenceDataStoreBase<TIntf, TImpl, TKey>: DataStoreBase, IReferenceDataStore<TIntf, TImpl, TKey>, IQueryableSortProvider<TImpl>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
	{
		private readonly Lazy<IIncludeExpressionRegistry<TImpl>> _lazyIncludeExpressionRegistry;

		protected ReferenceDataStoreBase()
		{
			_lazyIncludeExpressionRegistry = new Lazy<IIncludeExpressionRegistry<TImpl>>(() => new IncludeExpressionRegistry<TImpl, TImpl>(DbContextProviderResolver.GetProvider<TImpl>()));
		}

		/// <summary>
		///     Провайдер фильтров.
		/// </summary>
		public IDataFilterProvider DataFilterProvider { get; set; }

		public IPredicateBuilder PredicateBuilder { get; set; }

		public IDataFilter DataFilter { get; set; }

		/// <summary>
		///     Реестр Include выражений.
		/// </summary>
		public IIncludeExpressionRegistry<TImpl> IncludeExpressionRegistry => _lazyIncludeExpressionRegistry.Value;

		public ISpecificationBuildingContext SpecificationBuildingContext => LazyGetRequiredService<ISpecificationBuildingContext>();

		IQueryable<TImpl> IQueryableSortProvider<TImpl>.Apply(IQueryable<TImpl> query)
		{
			return DefaultSortQuery(query);
		}

		IQueryable IQueryableSortProvider.Apply(IQueryable query)
		{
			return DefaultSortQuery((IQueryable<TImpl>) query);
		}

		/// <summary>
		///     Выбирает из базы копию данных, доступную только для чтения.
		/// </summary>
		/// <param name="id">Идентификатор искомой записи.</param>
		/// <returns>Информация, доступная только для чтения.</returns>
		public virtual async Task<TIntf> Get(TKey id)
		{
			return await GetEditable(id);
		}

		/// <summary>
		///     Выбирает из базы несколько записей для указанного списка идентификаторв.
		/// </summary>
		/// <param name="idList">Список идентификаторов извлекаемых из база записей.</param>
		/// <returns>Список записей, доступных только для чтения.</returns>
		public virtual async Task<List<TIntf>> GetMany(params TKey[] idList)
		{
			IQueryable<TImpl> query = GetQuery();
			query = IncludeRelatedData(query);

			List<TImpl> result = await query
				.Where(e => idList.Contains(((IEntity<TKey>) e).Id))
				.ToListAsync();

			return result.Cast<TIntf>().ToList();
		}

		/// <summary>
		///     Возвращает страницу отсортированного списка данных.
		/// </summary>
		/// <param name="pageNumber">Номер страницы (нумерация страниц начинается с 1).</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="filterValues">Список значений фильтров для построения выражения фильтрации.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="countTotal">
		///     Флаг, включающий подсчет общего количества записей. Если содержит значение true, то
		///     выполняется дополнительная выборка из базы, выполняющая подсчет общего количества данных.
		/// </param>
		/// <returns>Данные о выбранной странице записей.</returns>
		public virtual async Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize,
			ICollection<DataFilterValue> filterValues = null, ICollection<SortOption> sortOptions = null, bool countTotal = false)
		{
			if (pageNumber < 1) throw new ArgumentException("Page number should be equal or greater then 1.", nameof(pageNumber));
			if (pageSize < 1) throw new ArgumentException("Page size should be equal or greater then 1.", nameof(pageSize));

			bool isIgnoreSafeDeleteFilter = false;
			bool isIgnoreMultiTenantFilter = false;
			if (filterValues != null)
			{
				List<IDataFilterDef> dataFilters = await DataFilterProvider.GetFilters(filterValues.Select(i => i.DataFilterId).ToArray());
				List<DataFilterOptions> dataFilterOptions = dataFilters.Select(dataFilter =>
						{
							Dictionary<string, object> filterOptions = DataFilterOptionsExtensions.Deserialize(dataFilter.Options);
							return new DataFilterOptions(Converter, filterOptions);
						})
					.ToList();

				isIgnoreSafeDeleteFilter = dataFilterOptions.Any(options => options.IsIgnoreSafeDeleteFilter);
				isIgnoreMultiTenantFilter = dataFilterOptions.Any(options => options.IsIgnoreMultiTenantFilter);
			}

			using (isIgnoreSafeDeleteFilter ? DataFilter.Disable<ISoftDelete>() : DataFilter.Enable<ISoftDelete>())
			using (isIgnoreMultiTenantFilter ? DataFilter.Disable<IMultiTenant>() : DataFilter.Enable<IMultiTenant>())
			{
				Expression<Func<TImpl, bool>> predicate = await DataFilterProvider.ToExpressionAsync<TImpl>(filterValues);
				return await GetPageInternal(pageNumber, pageSize, countTotal, predicate, sortOptions);
			}
		}

		/// <summary>
		///     Возвращает страницу отсортированного списка данных.
		/// </summary>
		/// <param name="pageNumber">Номер страницы (нумерация страниц начинается с 1).</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="predicate">Выражение для фильтрации данных.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="countTotal">
		///     Флаг, включающий подсчет общего количества записей. Если содержит значение true, то
		///     выполняется дополнительная выборка из базы, выполняющая подсчет общего количества данных.
		/// </param>
		/// <returns>Данные о выбранной странице записей.</returns>
		public virtual Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize,
			Expression<Func<TImpl, bool>> predicate, ICollection<SortOption> sortOptions = null, bool countTotal = false)
		{
			return GetPageInternal(pageNumber, pageSize, countTotal, predicate, sortOptions);
		}

		/// <inheritdoc
		///     cref="IReferenceDataStore{TIntf,TImpl,TKey}.GetPage(int,int,Expression{Func{TImpl,bool}},IOrderSpecification{TImpl},bool)" />
		public virtual Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, Expression<Func<TImpl, bool>> predicate,
			IOrderSpecification<TImpl> orderSpecification, bool countTotal = false)
		{
			return GetPageInternal(pageNumber, pageSize, countTotal, predicate, orderSpecification);
		}

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="filterSpecification">Спецификация для выборки данных (фильтры, сортировка и т.п.).</param>
		/// <param name="orderSpecification">Спецификация сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		public virtual Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, IFilterSpecification<TImpl> filterSpecification = null,
			IOrderSpecification<TImpl> orderSpecification = null, bool countTotal = false)
		{
			return GetPageInternal(pageNumber, pageSize, countTotal, filterSpecification, orderSpecification, false);
		}

		public virtual async Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, List<SpecificationParameters> specifications, bool countTotal = false)
		{
			List<ISpecification<TImpl>> allSpecs = SpecificationHelpers.BuildSpecifications<TImpl>(SpecificationBuildingContext, specifications);

			IFilterSpecification<TImpl> filterSpec = allSpecs.ToFilterSpecification();
			IOrderSpecification<TImpl> orderSpec = allSpecs.ToOrderSpecification();

			return await GetPage(pageNumber, pageSize, filterSpec, orderSpec, countTotal);
		}

		/// <summary>
		///     Все элементы подпадающие под условия фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="asNoTracking"></param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		public virtual async Task<List<TIntf>> GetAll(Expression<Func<TImpl, bool>> predicate = null,
			ICollection<SortOption> sortOptions = null, bool asNoTracking = false)
		{
			ResultSet<TIntf> resultSet = await GetPageInternal(1, int.MaxValue, predicate: predicate, sortOptions: sortOptions, asNoTracking: asNoTracking);
			return resultSet.Data;
		}

		/// <inheritdoc cref="IReferenceDataStore{TIntf,TImpl,TKey}.CountAsync" />
		public virtual async Task<int> CountAsync(Expression<Func<TImpl, bool>> predicate = null)
		{
			predicate = predicate ?? (e => true);
			return await GetQuery().Where(predicate).CountAsync();
		}

		/// <summary>
		///     Возвращает первый элемент подпадающий под условия фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		public virtual async Task<TIntf> GetFirst(Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null)
		{
			ResultSet<TIntf> resultSet = await GetPageInternal(1, 1, predicate: predicate, sortOptions: sortOptions);
			return resultSet.Data.FirstOrDefault();
		}

		/// <summary>
		///     Возвращает единственный (выполняется проверка что существует только один элемент) элемент подпадающий под условия
		///     фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		public virtual async Task<TIntf> GetSingle(Expression<Func<TImpl, bool>> predicate)
		{
			ResultSet<TIntf> resultSet = await GetPageInternal(1, 2, predicate: predicate);
			return resultSet.Data.SingleOrDefault();
		}

		/// <summary>
		///     Делает выборку данных и возвращает экземпляр класса, который можно редактировать.
		/// </summary>
		/// <param name="id">Идентификатор искомой записи.</param>
		/// <returns>Экземпляр класса, доступный для редактирования.</returns>
		public virtual Task<TImpl> GetEditable(TKey id)
		{
			IQueryable<TImpl> query = GetQuery();
			query = IncludeRelatedData(query);
			return query.FirstOrDefaultAsync(e => ((IEntity<TKey>) e).Id.Equals(id));
		}

		/// <summary>
		///     Приводит интерфейс к классу. Если приведение невозможно, то из базы извлекается копия, доступная для дальнейшего
		///     редактирования.
		/// </summary>
		/// <param name="entry">Исходные данные.</param>
		/// <returns>Экземпляр класса, доступный для редактирования.</returns>
		public virtual async Task<TImpl> GetEditable(TIntf entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			return entry as TImpl ?? await GetEditable(entry.Id);
		}

		public virtual TImpl AsEditable(TIntf entry)
		{
			return entry as TImpl ?? throw new InvalidOperationException();
		}

		public async Task<TIntf> LoadRelatedData(TIntf entry)
		{
			TImpl editable = AsEditable(entry);
			await EnsureLoaded(editable);
			return editable;
		}

		/// <summary>
		///     Возвращает объект, необходимый для построения запроса выборки данных из БД.
		/// </summary>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected abstract IQueryable<TImpl> GetQuery();

		/// <summary>
		///     Включает загрузку зарегистрированных ранее свойств содержащих ссылки на связанные объекти или коллекции объектов.
		/// </summary>
		/// <param name="query">Объект, необходимый для построения запроса выборки данных из БД.</param>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected virtual IQueryable<TImpl> IncludeRelatedData(IQueryable<TImpl> query)
		{
			return IncludeExpressionRegistry.Include(query);
		}

		/// <summary>
		///     Загружает все зарегистрированные свойства или списки свзязанных сущностей.
		/// </summary>
		/// <param name="existingEntry">Экземпляр существующей записи, извлеченной из БД.</param>
		/// <returns></returns>
		protected virtual async Task EnsureLoaded(TImpl existingEntry)
		{
			await IncludeExpressionRegistry.EnsureLoadedAsync(existingEntry);
		}

		/// <summary>
		///     Возвращает страницу отсортированного списка данных.
		/// </summary>
		/// <param name="pageNumber">Номер страницы (нумерация страниц начинается с 1).</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">
		///     Флаг, включающий подсчет общего количества записей. Если содержит значение true, то
		///     выполняется дополнительная выборка из базы, выполняющая подсчет общего количества данных.
		/// </param>
		/// <param name="predicate">Выражение для фильтрации данных.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="asNoTracking">Возвращает readonly сущности, по которым отслеживание изменений не выполняется.</param>
		/// <returns>Данные о выбранной странице записей.</returns>
		protected virtual async Task<ResultSet<TIntf>> GetPageInternal(int pageNumber, int pageSize, bool countTotal = false,
			Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null, bool asNoTracking = false)
		{
			predicate = predicate ?? (e => true);
			int? totalCount = countTotal ? await GetQuery().Where(predicate).CountAsync() : (int?) null;
			if (totalCount.HasValue)
				if (totalCount.Value <= (pageNumber - 1) * pageSize)
					return Enumerable.Empty<TIntf>().ToResultSet(pageNumber, pageSize, totalCount);
			IQueryableSortProvider<TImpl> sortProvider = sortOptions == null || sortOptions.Count == 0
					? (IQueryableSortProvider<TImpl>) this
					: new SortOptionsSortProvider<TImpl>(PredicateBuilder, sortOptions)
				;
			return await IncludeRelatedData(GetQuery())
				.Where(predicate)
				.OrderBy(sortProvider)
				.AsNoTracking(asNoTracking)
				.ToResultSetAsync(pageNumber, pageSize, totalCount)
				.Extensions().CastAsync<TIntf>();
		}

		/// <summary>
		///     Возвращает страницу отсортированного списка данных.
		/// </summary>
		/// <param name="pageNumber">Номер страницы (нумерация страниц начинается с 1).</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">
		///     Флаг, включающий подсчет общего количества записей. Если содержит значение true, то
		///     выполняется дополнительная выборка из базы, выполняющая подсчет общего количества данных.
		/// </param>
		/// <param name="predicate">Выражение для фильтрации данных.</param>
		/// <param name="orderSpecification">Спецификация сортировки.</param>
		/// <param name="asNoTracking">Возвращает readonly сущности, по которым отслеживание изменений не выполняется.</param>
		/// <returns>Данные о выбранной странице записей.</returns>
		protected virtual async Task<ResultSet<TIntf>> GetPageInternal(int pageNumber, int pageSize, bool countTotal,
			Expression<Func<TImpl, bool>> predicate, IOrderSpecification<TImpl> orderSpecification, bool asNoTracking = false)
		{
			if (pageNumber <= 0)
				throw new ArgumentException("Page number should be greater then zero.", nameof(pageNumber));
			if (pageSize <= 0)
				throw new ArgumentException("Page size should be greater then zero.", nameof(pageSize));

			orderSpecification = orderSpecification ?? new NoOrderBySpecification<TImpl>();

			int? totalCount = countTotal ? await GetQuery().Where(predicate).CountAsync() : (int?) null;
			if (totalCount.HasValue)
				if (totalCount.Value <= (pageNumber - 1) * pageSize)
					return Enumerable.Empty<TIntf>().ToResultSet(pageNumber, pageSize, totalCount);
			IQueryableSortProvider<TImpl> sortProvider = orderSpecification.IsEmpty
					? (IQueryableSortProvider<TImpl>) this
					: orderSpecification
				;
			return await IncludeRelatedData(GetQuery())
				.Where(predicate)
				.OrderBy(sortProvider)
				.AsNoTracking(asNoTracking)
				.ToResultSetAsync(pageNumber, pageSize, totalCount)
				.Extensions().CastAsync<TIntf>();
		}

		/// <summary>
		///     Возвращает страницу отсортированного списка данных.
		/// </summary>
		/// <param name="pageNumber">Номер страницы (нумерация страниц начинается с 1).</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">
		///     Флаг, включающий подсчет общего количества записей. Если содержит значение true, то
		///     выполняется дополнительная выборка из базы, выполняющая подсчет общего количества данных.
		/// </param>
		/// <param name="filterSpecification">Спецификация для фильтрации данных.</param>
		/// <param name="orderSpecification">Спецификация сортировки.</param>
		/// <param name="asNoTracking">Возвращает readonly сущности, по которым отслеживание изменений не выполняется.</param>
		/// <returns>Данные о выбранной странице записей.</returns>
		protected virtual async Task<ResultSet<TIntf>> GetPageInternal(int pageNumber, int pageSize, bool countTotal = false,
			IFilterSpecification<TImpl> filterSpecification = null, IOrderSpecification<TImpl> orderSpecification = null,
			bool asNoTracking = false)
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

				int? totalCount = countTotal ? await GetQuery().Where(predicate).CountAsync() : (int?) null;
				if (totalCount.HasValue)
					if (totalCount.Value <= (pageNumber - 1) * pageSize)
						return Enumerable.Empty<TIntf>().ToResultSet(pageNumber, pageSize, totalCount);
				IQueryableSortProvider<TImpl> sortProvider = orderSpecification.IsEmpty
						? (IQueryableSortProvider<TImpl>) this
						: orderSpecification
					;
				return await IncludeRelatedData(GetQuery())
					.Where(predicate)
					.OrderBy(sortProvider)
					.AsNoTracking(asNoTracking)
					.ToResultSetAsync(pageNumber, pageSize, totalCount)
					.Extensions().CastAsync<TIntf>();
			}
		}

		/// <summary>
		///     Включает сортировку выбираемых данных. По-умолчанию сортировка по идентификатору. Сортировку можно переопределить в
		///     наследниках.
		/// </summary>
		/// <param name="query">Объект, необходимый для построения запроса выборки данных из БД.</param>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected virtual IQueryable<TImpl> DefaultSortQuery(IQueryable<TImpl> query)
		{
			return query
					.OrderBy(e => ((IEntity<TKey>) e).Id)
				;
		}
	}

	public abstract class ReferenceDataStoreBase<TIntf, TImpl, TKey, TDataRepository>: ReferenceDataStoreBase<TIntf, TImpl, TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDataRepository: class, IReadOnlyRepository<TImpl, TKey>
	{
		/// <summary>
		///     Репозиторий для доступа к БД.
		/// </summary>
		protected TDataRepository DataRepository => LazyGetRequiredService<TDataRepository>();

		/// <summary>
		///     Возвращает объект, необходимый для построения запроса выборки данных из БД.
		/// </summary>
		/// <returns>Объект, необходимый для построения запроса выборки данных из БД.</returns>
		protected override IQueryable<TImpl> GetQuery()
		{
			return DataRepository;
		}
	}
}
