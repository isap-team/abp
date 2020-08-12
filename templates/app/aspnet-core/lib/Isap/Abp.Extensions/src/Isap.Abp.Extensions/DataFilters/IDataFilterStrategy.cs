using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterStrategy
	{
		Expression CreateFilterExpression(Dictionary<string, object> options);
		Expression CreateFilterExpression(JToken options);
		Expression CreateFilterExpression(string options);
		Dictionary<string, object> CompleteOptions(Dictionary<string, object> options);
		JToken CompleteOptions(JToken options);
	}

	public interface IDataFilterStrategy<TEntity>: IDataFilterStrategy
	{
		new Expression<Func<TEntity, bool>> CreateFilterExpression(Dictionary<string, object> options);
		new Expression<Func<TEntity, bool>> CreateFilterExpression(JToken options);
		new Expression<Func<TEntity, bool>> CreateFilterExpression(string options);
	}
}
