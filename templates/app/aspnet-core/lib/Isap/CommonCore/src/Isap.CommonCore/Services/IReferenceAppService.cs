using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isap.CommonCore.Services
{
	public interface IReferenceAppService<TEntityDto, TKey>: ICommonAppService
		where TEntityDto: ICommonEntityDto<TKey>
	{
		/// <summary>
		///     Возвращает информацию об элементе сущности.
		/// </summary>
		/// <param name="id">Идентификатор записи.</param>
		/// <returns>Ссылка на экземпляр информации о сущности.</returns>
		Task<TEntityDto> Get(TKey id);

		/// <summary>
		///     Возвращает список запрошенных элементов сущности по указанным идентификаторам.
		/// </summary>
		/// <param name="idList"></param>
		/// <returns></returns>
		Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList);

		/// <summary>
		///     Возвращает страницу списка элементов сущности.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <param name="queryOptions">Параметры запроса: фильтры, сортировка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		//[Obsolete("Use GetPage(int, int, bool, List<SpecificationParameters>) overload.")]
		Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null);

		/// <summary>
		///     Возвращает страницу списка элементов сущности.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <param name="specifications">Спецификации запроса: фильтры, сортировка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal, List<SpecificationParameters> specifications);

		/// <summary>
		///     Возвращает страницу списка элементов сущности.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <param name="filterValues">Коллекция значений фильтров при выборке списка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		[Obsolete("Use GetPage(int, int, bool, QueryOptionsInput) overload.")]
		Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal, ICollection<DataFilterValueDto> filterValues);

		/// <summary>
		///     Возвращает страницу списка элементов сущности.
		/// </summary>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="countTotal">Флаг управления подсчетом общего количества записей.</param>
		/// <param name="queryOptions">Параметры запроса: фильтры, сортировка.</param>
		/// <returns>Информация о странице, выбранной из базы данных.</returns>
		[Obsolete("Use GetPage(int, int, bool, QueryOptionsInput) overload.")]
		Task<ResultSet<TEntityDto>> QueryPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null);
	}
}
