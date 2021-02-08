using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterProvider
	{
		/// <summary>
		///     Возвращает список записей с информацией о динамических фильтрах данных.
		/// </summary>
		/// <param name="idList">Список идентификаторов загружаемых записей.</param>
		/// <returns>Список найденных записей.</returns>
		Task<List<IDataFilterDef>> GetFilters(params Guid[] idList);

		/// <summary>
		///     Возвращает выражение, которое можно использовать в ORM для построения SQL выражений по условиям поиска.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		Task<Expression<Func<TEntity, bool>>> ToExpressionAsync<TEntity>(ICollection<DataFilterValue> filterValues);
	}
}
