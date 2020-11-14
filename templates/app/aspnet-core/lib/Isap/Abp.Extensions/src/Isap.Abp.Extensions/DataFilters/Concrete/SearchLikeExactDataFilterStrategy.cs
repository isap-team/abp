using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SearchLikeExactDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SearchLikeExactDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var selectLikeOptions = new SearchLikeDataFilterOptions(Converter, options);
			List<string> searchValues = selectLikeOptions.Value.AsEmptyIfNull().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.TrimStart())
				.ToList();
			switch (searchValues.Count)
			{
				case 0:
					return PredicateBuilder.True<TEntity>();
				case 1:
					return CreateExpression(selectLikeOptions, searchValues[0]);
				default:
					return searchValues.Aggregate(PredicateBuilder.False<TEntity>(), (expr, searchValue) =>
						expr.OrElse(CreateExpression(selectLikeOptions, searchValue))
					);
			}
		}

		private Expression<Func<TEntity, bool>> CreateExpression(SearchLikeDataFilterOptions options, string searchValue)
		{
			if (searchValue.IndexOf('@') >= 0 && !options.EmailProperty.IsNullOrEmpty())
			{
				if (!searchValue.EndsWith("%"))
					searchValue = (searchValue + "%").Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).First();
				return CreateExpression(options.CaseSensitive, options.EmailProperty, searchValue);
			}

			if (options.UseSearchByFullName)
			{
				if (FullName.TryParseForSearch(searchValue, out FullName fullName))
				{
					Expression<Func<TEntity, bool>> expression;
					bool searchByFullNameAlso = false;
					while (true)
					{
						if (options.LastNameProperty.IsNullOrEmpty())
						{
							// Мы не можем искать по фамилии, потому что не указано поле, содержащее эти данные, но тогда нужно включить поиск по полному имени.
							searchByFullNameAlso = true;
							expression = PredicateBuilder.True<TEntity>();
						}
						else
						{
							expression = CreateExpression(options.CaseSensitive, options.LastNameProperty, fullName.LastName);
						}

						if (fullName.FirstName.IsNullOrEmpty())
						{
							fullName.FirstName = "%";
						}

						if (options.FirstNameProperty.IsNullOrEmpty())
						{
							// Мы не можем искать по имени, потому что не указано поле, содержащее эти данные, но тогда нужно включить поиск по полному имени.
							searchByFullNameAlso = true;
						}
						else
						{
							expression = expression.AndAlso(CreateExpression(options.CaseSensitive, options.FirstNameProperty, fullName.FirstName));
						}

						if (fullName.MiddleName.IsNullOrEmpty())
							break;

						if (options.MiddleNameProperty.IsNullOrEmpty())
						{
							// Мы не можем искать по отчеству, потому что не указано поле, содержащее эти данные, но тогда нужно включить поиск по полному имени.
							searchByFullNameAlso = true;
						}
						else
						{
							expression = expression.AndAlso(CreateExpression(options.CaseSensitive, options.MiddleNameProperty, fullName.MiddleName));
						}

						break;
					}

					if (searchByFullNameAlso)
						expression = expression.AndAlso(CreateExpression(options.CaseSensitive, options.PropertyName,
							$"{fullName.LastName} {fullName.FirstName} {fullName.MiddleName}".Trim()));

					expression = CompleteExpression(options, searchValue, expression);

					return expression;
				}
			}

			return CompleteExpression(options, searchValue, CreateExpression(options.CaseSensitive, options.PropertyName, $"%{searchValue}%"));
		}

		private Expression<Func<TEntity, bool>> CompleteExpression(SearchLikeDataFilterOptions options, string searchValue,
			Expression<Func<TEntity, bool>> expression)
		{
			if (searchValue.EndsWith(" "))
			{
				string trimmedValue = searchValue.Trim();
				return CreateExpression(options.CaseSensitive, options.PropertyName, $"{trimmedValue} %")
					.OrElse(CreateExpression(options.CaseSensitive, options.PropertyName, $"% {trimmedValue}"))
					.OrElse(CreateExpression(options.CaseSensitive, options.PropertyName, $"% {trimmedValue} %"));
			}

			return expression;
		}

		private Expression<Func<TEntity, bool>> CreateExpression(bool caseSensitive, string propertyName, string searchExpr)
		{
			searchExpr = searchExpr.Replace("'", "''"); // устранение возможности SQL-инъекции
			return caseSensitive
					? PredicateBuilder.Like<TEntity>(propertyName, searchExpr)
					: PredicateBuilder.ILike<TEntity>(propertyName, searchExpr)
				;
		}
	}
}
