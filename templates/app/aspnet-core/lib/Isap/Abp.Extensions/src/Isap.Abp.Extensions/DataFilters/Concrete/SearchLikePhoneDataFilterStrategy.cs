using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.CommonCore.Extensions;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SearchLikePhoneDataFilterOptions: DataFilterOptions
	{
		public SearchLikePhoneDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			Value = converter.ConvertTo<string>(options.GetOrDefault(nameof(Value)));
			CaseSensitive = converter.ConvertTo<bool>(options.GetOrDefault(nameof(CaseSensitive)));
			ContainsInAnyField = converter.ConvertTo<bool>(options.GetOrDefault(nameof(ContainsInAnyField)));
			PropertiesName = converter.ConvertTo<string[]>(options.GetOrDefault(nameof(PropertiesName)));
		}

		/// <summary>
		///     Конфигурация поискового запроса.
		/// </summary>
		/// <param name="containsInAnyField">
		///     Использовать оператор Or для построения поисковой цепочки. По умолчанию используется
		///     And.
		/// </param>
		/// <param name="propertiesName">Одно или несколько полей по которым возможен поиск.</param>
		/// <param name="caseSensitive">Учитывать регистр.</param>
		public SearchLikePhoneDataFilterOptions(bool containsInAnyField, string[] propertiesName, bool caseSensitive = false)
			: base(propertiesName[0], false, "TextInput")
		{
			ContainsInAnyField = containsInAnyField;
			PropertiesName = propertiesName;
			CaseSensitive = caseSensitive;
		}

		public string Value { get; set; }
		public bool ContainsInAnyField { get; set; }
		public string[] PropertiesName { get; set; }
		public bool CaseSensitive { get; set; }
	}

	/// <summary>
	///     Поиск с учетом положения пробела.
	/// </summary>
	/// <typeparam name="TEntity">Целевая сущность</typeparam>
	public class SearchLikePhoneDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SearchLikePhoneDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var selectLikeOptions = new SearchLikePhoneDataFilterOptions(Converter, options);
			string searchValue = selectLikeOptions.Value.AsEmptyIfNull();

			switch (searchValue)
			{
				case { } dummy when searchValue.StartsWith(" ") && searchValue.EndsWith(" "):
					searchValue = searchValue.Trim();
					break;
				case { } dummy when searchValue.StartsWith(" "):
					searchValue = $"{searchValue.TrimStart()}%";
					break;
				case { } dummy when searchValue.EndsWith(" "):
					searchValue = $"%{searchValue.TrimEnd()}";
					break;
				default:
					searchValue = $"%{searchValue}%";
					break;
			}

			return CreateSpaceExpression(selectLikeOptions.PropertiesName, searchValue, selectLikeOptions.ContainsInAnyField, selectLikeOptions.CaseSensitive);
		}

		private Expression<Func<TEntity, bool>> CreateSpaceExpression(string[] propertiesName, string searchValue, bool containsInAnyField, bool caseSensitive)
		{
			Expression<Func<TEntity, bool>> resultExpression = CreateExpression(propertiesName[0], searchValue, caseSensitive);
			for (int i = 1; i < propertiesName.Length; i++)
				resultExpression = containsInAnyField
					? resultExpression.OrElse(CreateExpression(propertiesName[i], searchValue, caseSensitive))
					: resultExpression.AndAlso(CreateExpression(propertiesName[i], searchValue, caseSensitive));

			return resultExpression;
		}

		private Expression<Func<TEntity, bool>> CreateExpression(string propertyName, string searchExpr, bool caseSensitive)
		{
			searchExpr = searchExpr.Replace("'", "''");
			return caseSensitive
					? PredicateBuilder.Like<TEntity>(propertyName, searchExpr)
					: PredicateBuilder.ILike<TEntity>(propertyName, searchExpr)
				;
		}
	}
}
