using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SelectOneDataFilterOptions: DataFilterOptions
	{
		public SelectOneDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			Value = options.GetOrDefault(nameof(Value));
			AutoCompleteFilterId = converter.TryConvertTo<Guid?>(options.GetOrDefault(nameof(AutoCompleteFilterId))).AsDefaultIfNotSuccess();
		}

		public SelectOneDataFilterOptions(string propertyName, string inputType = null, object value = null, Guid? entityId = null,
			string enumType = null, List<DataFilterEnumValue> enumValues = null, bool isInverted = false, Guid? autoCompleteFilterId = null)
			: base(propertyName, isInverted, inputType: inputType, entityId: entityId, enumType: enumType, enumValues: enumValues)
		{
			Value = value;
			AutoCompleteFilterId = autoCompleteFilterId;
		}

		public SelectOneDataFilterOptions()
		{
		}

		public object Value { get; set; }
		public Guid? AutoCompleteFilterId { get; }
	}

	public class SelectOneDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SelectOneDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var optionsWrapper = new SelectOneDataFilterOptions(Converter, options);
			//if (IsString(optionsWrapper.Value, out string searchExpr))
			//	return PredicateBuilder.ILike<TEntity>(optionsWrapper.PropertyName, searchExpr);
			return PredicateBuilder.Equal<TEntity>(optionsWrapper.PropertyName, optionsWrapper.Value);
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
