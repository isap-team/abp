using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class IsAnyOrRangeDataFilterOptions: DataFilterOptions
	{
		public IsAnyOrRangeDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			BooleanPropertyName = converter.TryConvertTo<string>(options.GetOrDefault(nameof(BooleanPropertyName))).AsDefaultIfNotSuccess();
			BooleanValue = converter.TryConvertTo<bool?>(options.GetOrDefault(nameof(BooleanValue))).AsDefaultIfNotSuccess();
			FromValue = options.GetOrDefault(nameof(FromValue));
			ToValue = options.GetOrDefault(nameof(ToValue));
			DefaultFromValue = options.GetOrDefault(nameof(DefaultFromValue));
			DefaultToValue = options.GetOrDefault(nameof(DefaultToValue));
		}

		public IsAnyOrRangeDataFilterOptions(string booleanPropertyName, string propertyName, string inputType, bool isInverted = false,
			bool? booleanValue = null, object fromValue = null, object toValue = null, object defaultFromValue = null, object defaultToValue = null,
			Guid? entityId = null, string enumType = null, List<DataFilterEnumValue> enumValues = null)
			: base(propertyName, isInverted, inputType: inputType, entityId: entityId, enumType: enumType, enumValues: enumValues)
		{
			BooleanPropertyName = booleanPropertyName;
			BooleanValue = booleanValue;
			FromValue = fromValue;
			ToValue = toValue;
			DefaultFromValue = defaultFromValue;
			DefaultToValue = defaultToValue;
		}

		public IsAnyOrRangeDataFilterOptions()
		{
		}

		public string BooleanPropertyName { get; set; }

		public bool? BooleanValue { get; set; }
		public object FromValue { get; set; }
		public object ToValue { get; set; }
		public object DefaultFromValue { get; }
		public object DefaultToValue { get; }
	}

	public class IsAnyOrRangeDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public IsAnyOrRangeDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			throw new NotImplementedException();
		}
	}
}
