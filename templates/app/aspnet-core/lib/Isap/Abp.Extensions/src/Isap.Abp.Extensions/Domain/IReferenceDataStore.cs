using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data.Specifications.FilterSpecs;
using Isap.Abp.Extensions.Data.Specifications.OrderSpecs;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.Querying;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using JetBrains.Annotations;

namespace Isap.Abp.Extensions.Domain
{
	public interface IReferenceDataStore<TIntf, in TKey>
		where TIntf: class
	{
		/// <summary>
		///     Возвращает информацию (только для чтения) о сущности.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns>Ссылка на экземпляр информации о сущности.</returns>
		Task<TIntf> Get(TKey id);

		/// <summary>
		///     Возвращает список запрошенных элементов сущности.
		/// </summary>
		/// <param name="idList">Список идентификаторов загружаемых записей.</param>
		/// <returns>Список найденных записей.</returns>
		Task<List<TIntf>> GetMany(params TKey[] idList);

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="filterValues">Набор значений для динамической фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns></returns>
		Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, ICollection<DataFilterValue> filterValues = null,
			ICollection<SortOption> sortOptions = null, bool countTotal = false);
	}

	public interface IReferenceDataStore<TIntf, TImpl, in TKey>: IReferenceDataStore<TIntf, TKey>
		where TIntf: class, ICommonEntity<TKey>
		where TImpl: class, TIntf
	{
		//IIsapDbContextProvider DbContextProvider { get; }

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, Expression<Func<TImpl, bool>> predicate,
			ICollection<SortOption> sortOptions = null, bool countTotal = false);

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="orderSpecification">Спецификация сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, Expression<Func<TImpl, bool>> predicate,
			IOrderSpecification<TImpl> orderSpecification, bool countTotal = false);

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="filterSpecification">Спецификация для выборки данных (фильтры, сортировка и т.п.).</param>
		/// <param name="orderSpecification">Спецификация сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, IFilterSpecification<TImpl> filterSpecification = null,
			IOrderSpecification<TImpl> orderSpecification = null, bool countTotal = false);

		/// <summary>
		///     Возвращает страницу списка.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="specifications">Спецификации фильтрации и сортировки</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<ResultSet<TIntf>> GetPage(int pageNumber, int pageSize, List<SpecificationParameters> specifications, bool countTotal = false);

		/// <summary>
		///     Возвращает все элементы подпадающие под условия фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <param name="asNoTracking"></param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<List<TIntf>> GetAll(Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null, bool asNoTracking = false);

		/// <summary>
		///     Возвращает количество записей, отвечающих критериям поиска.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		Task<int> CountAsync(Expression<Func<TImpl, bool>> predicate = null);

		/// <summary>
		///     Возвращает первый элемент подпадающий под условия фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <param name="sortOptions">Параметры сортировки</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<TIntf> GetFirst(Expression<Func<TImpl, bool>> predicate = null, ICollection<SortOption> sortOptions = null);

		/// <summary>
		///     Возвращает единственный (выполняется проверка что существует только один элемент) элемент подпадающий под условия
		///     фильтрации.
		/// </summary>
		/// <param name="predicate">Выражение для фильтрации списка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<TIntf> GetSingle(Expression<Func<TImpl, bool>> predicate);

		/// <summary>
		///     Возвращает ссылку на редактируемый экземпляр сущности, полученной из БД.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns></returns>
		Task<TImpl> GetEditable(TKey id);

		/// <summary>
		///     Возвращает ссылку на редактируемый экземпляр сущности, преобразованной или полученной из БД.
		/// </summary>
		/// <param name="entry">Ссылка на информацию только для чтения.</param>
		/// <returns></returns>
		Task<TImpl> GetEditable([NotNull] TIntf entry);

		/// <summary>
		///     Преобразует к редактируемому типу или создает новую сущность по шаблону.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		TImpl AsEditable([NotNull] TIntf entry);

		/// <summary>
		///     Подгружает связанные данные согласно определенным правилам менеджера.
		/// </summary>
		/// <param name="entry">Ссылка на информацию только для чтения.</param>
		/// <returns></returns>
		Task<TIntf> LoadRelatedData([NotNull] TIntf entry);
	}
}
