using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isap.Abp.Extensions.DataFilters.Converters;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Expressions.Predicates;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.DataFilters
{
	public class DataFilterProvider: DomainServiceBase, IDataFilterProvider
	{
		public IPredicateBuilder PredicateBuilder { get; set; }
		public IObjectAccessor<IDataFilterDataStore> DataStore { get; set; }
		public IDataFilterValueConverterFactory DataFilterValueConverterFactory { get; set; }
		public ICustomPredicateBuilderFactory CustomPredicateBuilderFactory { get; set; }

		public async Task<List<IDataFilterDef>> GetFilters(params Guid[] idList)
		{
			List<IDataFilterDefinition> definitions = await DataStore.Value.GetMany(idList);
			return definitions.Cast<IDataFilterDef>().ToList();
		}

		public async Task<Expression<Func<TEntity, bool>>> ToExpressionAsync<TEntity>(ICollection<DataFilterValue> filterValues)
		{
			Expression<Func<TEntity, bool>> predicate = e => true;
			if (filterValues?.Count > 0)
			{
				Dictionary<Guid, DataFilterValue> dataFilterMap = filterValues.ToDictionary(i => i.DataFilterId);
				List<IDataFilterDef> dataFilters = await GetFilters(dataFilterMap.Keys.ToArray());
				List<Tuple<IDataFilterDef, string>> tuples = dataFilterMap
					.Join(dataFilters.Where(e => !e.IsDisabled), pair => pair.Key, e => e.Id, (pair, e) => Tuple.Create(e, pair.Value.Values))
					.ToList();
				foreach (Tuple<IDataFilterDef, string> tuple in tuples)
				{
					IDataFilterDef dataFilter = tuple.Item1;
					IDataFilterStrategyFactory<TEntity> dataFilterStrategyFactory =
						DataFilterStrategyFactory.GetOrCreate<TEntity>(Converter, PredicateBuilder, DataFilterValueConverterFactory,
							CustomPredicateBuilderFactory);
					IDataFilterStrategy<TEntity> dataFilterStrategy = dataFilterStrategyFactory.Create(dataFilter);
					Dictionary<string, object> filterOptions =
						DataFilterOptionsExtensions.Deserialize(dataFilter.Options).Replace(DataFilterOptionsExtensions.Deserialize(tuple.Item2));
					var expression = dataFilterStrategy.CreateFilterExpression(filterOptions);
					predicate = predicate.AndAlso(expression);
				}
			}

			return predicate;
		}
	}
}
