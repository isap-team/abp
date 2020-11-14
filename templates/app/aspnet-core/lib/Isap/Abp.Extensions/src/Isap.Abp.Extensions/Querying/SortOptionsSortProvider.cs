using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;

namespace Isap.Abp.Extensions.Querying
{
	public class SortOptionsSortProvider<TImpl>: IQueryableSortProvider<TImpl>, IEnumerableSortProvider<TImpl>
	{
		public SortOptionsSortProvider(IPredicateBuilder predicateBuilder, params SortOption[] sortOptions)
		{
			PredicateBuilder = predicateBuilder;
			SortOptions = new ReadOnlyCollection<SortOption>(sortOptions);
			//PredicateBuilder = DefaultPredicateBuilder.Instance;
		}

		public SortOptionsSortProvider(IPredicateBuilder predicateBuilder, ICollection<SortOption> sortOptions)
		{
			PredicateBuilder = predicateBuilder;
			SortOptions = sortOptions;
			//PredicateBuilder = DefaultPredicateBuilder.Instance;
		}

		public ICollection<SortOption> SortOptions { get; }

		public IPredicateBuilder PredicateBuilder { get; set; }

		public IEnumerable<TImpl> Apply(IEnumerable<TImpl> query)
		{
			ParameterExpression orderByParameter = Expression.Parameter(typeof(TImpl), "i");

			SortOption option = SortOptions.First();
			ISortExpressionProvider<TImpl> sortExpressionProvider = CreateSortExpressionProvider(orderByParameter, option.FieldName);
			IOrderedEnumerable<TImpl> orderedEnumerable = sortExpressionProvider.OrderBy(query, option.IsDescending);

			foreach (SortOption sortOption in SortOptions.Skip(1))
			{
				sortExpressionProvider = CreateSortExpressionProvider(orderByParameter, sortOption.FieldName);
				orderedEnumerable = sortExpressionProvider.ThenBy(orderedEnumerable, sortOption.IsDescending);
			}

			return orderedEnumerable;
		}

		IEnumerable IEnumerableSortProvider.Apply(IEnumerable query)
		{
			return Apply((IEnumerable<TImpl>) query);
		}

		public IQueryable<TImpl> Apply(IQueryable<TImpl> query)
		{
			ParameterExpression orderByParameter = Expression.Parameter(typeof(TImpl), "e");

			SortOption option = SortOptions.First();
			ISortExpressionProvider<TImpl> sortExpressionProvider = CreateSortExpressionProvider(orderByParameter, option.FieldName);
			IOrderedQueryable<TImpl> orderedQueryable = sortExpressionProvider.OrderBy(query, option.IsDescending);

			foreach (SortOption sortOption in SortOptions.Skip(1))
			{
				sortExpressionProvider = CreateSortExpressionProvider(orderByParameter, sortOption.FieldName);
				orderedQueryable = sortExpressionProvider.ThenBy(orderedQueryable, sortOption.IsDescending);
			}

			return orderedQueryable;
		}

		IQueryable IQueryableSortProvider.Apply(IQueryable query)
		{
			return Apply((IQueryable<TImpl>) query);
		}

		private ISortExpressionProvider<TImpl> CreateSortExpressionProvider(ParameterExpression orderByParameter, string fieldName)
		{
			MemberExpression memberExpression = PredicateBuilder.CreateMemberExpression(orderByParameter, fieldName);
			return (ISortExpressionProvider<TImpl>) Activator.CreateInstance(
				typeof(SortExpressionProvider<,>).MakeGenericType(typeof(TImpl), memberExpression.Type),
				memberExpression, orderByParameter);
		}
	}
}
