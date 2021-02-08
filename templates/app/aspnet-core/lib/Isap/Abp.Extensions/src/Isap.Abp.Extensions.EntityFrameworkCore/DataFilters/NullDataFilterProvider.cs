using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Expressions.Predicates;

namespace Isap.Abp.Extensions.DataFilters
{
	public class NullDataFilterProvider: IDataFilterProvider
	{
		public static IDataFilterProvider Instance = new NullDataFilterProvider();

		private NullDataFilterProvider()
		{
		}

		public Task<List<IDataFilterDef>> GetFilters(params Guid[] idList)
		{
			return Task.FromResult(new List<IDataFilterDef>());
		}

		public Task<Expression<Func<TEntity, bool>>> ToExpressionAsync<TEntity>(ICollection<DataFilterValue> filterValues)
		{
			return Task.FromResult(DefaultPredicateBuilder.Instance.True<TEntity>());
		}
	}
}
