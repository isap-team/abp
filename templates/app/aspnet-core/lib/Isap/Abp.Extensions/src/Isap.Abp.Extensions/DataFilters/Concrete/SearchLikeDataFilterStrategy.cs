using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Isap.Abp.Extensions.DataFilters.Converters;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SearchLikeDataFilterOptions: DataFilterOptions
	{
		public SearchLikeDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			CaseSensitive = converter.ConvertTo<bool>(options.GetOrDefault(nameof(CaseSensitive)));
			Value = converter.ConvertTo<string>(options.GetOrDefault(nameof(Value)));
			EmailProperty = converter.ConvertTo<string>(options.GetOrDefault(nameof(EmailProperty)));
			UseSearchByFullName = converter.TryConvertTo<bool>(options.GetOrDefault(nameof(UseSearchByFullName))).AsDefaultIfNotSuccess();
			FirstNameProperty = converter.ConvertTo<string>(options.GetOrDefault(nameof(FirstNameProperty)));
			MiddleNameProperty = converter.ConvertTo<string>(options.GetOrDefault(nameof(MiddleNameProperty)));
			LastNameProperty = converter.ConvertTo<string>(options.GetOrDefault(nameof(LastNameProperty)));
		}

		public SearchLikeDataFilterOptions(string propertyName, string inputType = null, string value = null, bool caseSensitive = false,
			bool isInverted = false, string searchValueConverterName = null, string customPredicateBuilderName = null)
			: base(propertyName, isInverted, inputType: inputType, searchValueConverterName: searchValueConverterName,
				customPredicateBuilderName: customPredicateBuilderName)
		{
			Value = value;
			CaseSensitive = caseSensitive;
		}

		public bool CaseSensitive { get; set; }
		public string Value { get; set; }

		public string EmailProperty { get; set; }

		public bool UseSearchByFullName { get; set; }

		public string FirstNameProperty { get; set; }
		public string MiddleNameProperty { get; set; }
		public string LastNameProperty { get; set; }
	}

	public class SearchLikeDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SearchLikeDataFilterStrategy(Dictionary<string, object> options)
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
			object valueToSearch = searchValue;
			if (options.SearchValueConverterName != null)
			{
				IDataFilterValueConverter searchValueConverter = DataFilterValueConverterFactory.Create(options.SearchValueConverterName);
				valueToSearch = searchValueConverter.Convert(valueToSearch);
			}

			if (options.CustomPredicateBuilderName != null)
			{
				var customPredicateBuilder = (ICustomPredicateBuilder<TEntity>) CustomPredicateBuilderFactory.Create(options.CustomPredicateBuilderName);
				return customPredicateBuilder.Build(valueToSearch);
			}

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
							if (fullName.FirstName.IsNullOrEmpty() && fullName.LastName.EndsWith("%"))
								expression = CreateExpression(options.CaseSensitive, options.LastNameProperty, fullName.LastName.EnsurePrefix("%"))
										.OrElse(CreateExpression(options.CaseSensitive, options.FirstNameProperty, fullName.LastName.EnsurePrefix("%")))
										.OrElse(CreateExpression(options.CaseSensitive, options.MiddleNameProperty, fullName.LastName.EnsurePrefix("%")))
									;
							else
								expression = CreateExpression(options.CaseSensitive, options.LastNameProperty, fullName.LastName);
						}

						if (fullName.FirstName.IsNullOrEmpty())
							break;

						if (options.FirstNameProperty.IsNullOrEmpty())
						{
							// Мы не можем искать по имени, потому что не указано поле, содержащее эти данные, но тогда нужно включить поиск по полному имени.
							searchByFullNameAlso = true;
						}
						else
						{
							expression = expression.AndAlso(fullName.MiddleName.IsNullOrEmpty()
									? CreateExpression(options.CaseSensitive, options.FirstNameProperty, fullName.FirstName)
										.OrElse(CreateExpression(options.CaseSensitive, options.MiddleNameProperty, fullName.FirstName))
									: CreateExpression(options.CaseSensitive, options.FirstNameProperty, fullName.FirstName))
								;
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

					return expression;
				}
			}

			return CreateExpression(options.CaseSensitive, options.PropertyName, $"%{searchValue}%");
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
