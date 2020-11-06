using System;
using System.Linq.Expressions;

namespace Isap.Abp.Extensions.DataFilters.Converters
{
	public interface ICustomPredicateBuilder
	{
		Expression Build(object value);
	}

	public interface ICustomPredicateBuilder<TEntity>: ICustomPredicateBuilder
	{
		new Expression<Func<TEntity, bool>> Build(object value);
	}
}
