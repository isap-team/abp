using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Isap.Abp.Extensions.Expressions.Predicates;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class IsAnyOrOneDataFilterOptions: DataFilterOptions
	{
		public IsAnyOrOneDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			BooleanPropertyName = converter.TryConvertTo<string>(options.GetOrDefault(nameof(BooleanPropertyName))).AsDefaultIfNotSuccess();
			BooleanValue = converter.TryConvertTo<bool?>(options.GetOrDefault(nameof(BooleanValue))).AsDefaultIfNotSuccess();
			Value = options.GetOrDefault(nameof(Value));
		}

		public IsAnyOrOneDataFilterOptions(string booleanPropertyName, string propertyName, bool isInverted = false, bool? booleanValue = null,
			object value = null,
			string inputType = null, Guid? entityId = null, string enumType = null,
			List<DataFilterEnumValue> enumValues = null)
			: base(propertyName, isInverted, inputType, entityId, enumType, enumValues)
		{
			BooleanPropertyName = booleanPropertyName;
			BooleanValue = booleanValue;
			Value = value;
		}

		public IsAnyOrOneDataFilterOptions()
		{
		}

		public string BooleanPropertyName { get; set; }

		public bool? BooleanValue { get; set; }
		public object Value { get; set; }
	}

	public class IsAnyOrOneDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public IsAnyOrOneDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var optionsWrapper = new IsAnyOrOneDataFilterOptions(Converter, options);
			if (!optionsWrapper.BooleanValue.HasValue)
				return PredicateBuilder.True<TEntity>();
			bool booleanValue = optionsWrapper.BooleanValue.Value;
			var expression = PredicateBuilder.Equal<TEntity>(optionsWrapper.BooleanPropertyName, booleanValue);
			if (booleanValue && optionsWrapper.Value != null)
			{
				Expression<Func<TEntity, bool>> secondExpression = /*IsString(optionsWrapper.Value, out string searchExpr)
						? PredicateBuilder.ILike<TEntity>(optionsWrapper.PropertyName, searchExpr)
						: */PredicateBuilder.Equal<TEntity>(optionsWrapper.PropertyName, optionsWrapper.Value)
					;
				expression = expression.AndAlso(secondExpression);
			}

			return expression;
		}

		public override Dictionary<string, object> CompleteOptions(Dictionary<string, object> options)
		{
			options = base.CompleteOptions(options);
			var filterOptions = new SelectOneDataFilterOptions(Converter, options);
			if (!string.IsNullOrEmpty(filterOptions.EnumType))
			{
				Type enumType = Type.GetType(filterOptions.EnumType);
				Debug.Assert(enumType != null);
				filterOptions.EnumValues = Enum.GetValues(enumType).Cast<int>()
					.Zip(Enum.GetNames(enumType), (value, name) => new DataFilterEnumValue(value.ToString(), name))
					.ToList();
				options = DataFilterOptionsExtensions.Deserialize(filterOptions.Serialize());
			}

			return options;
		}
	}
}
