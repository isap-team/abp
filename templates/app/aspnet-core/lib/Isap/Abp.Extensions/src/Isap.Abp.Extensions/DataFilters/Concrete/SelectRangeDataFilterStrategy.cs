using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SelectRangeDataFilterOptions: DataFilterOptions
	{
		public SelectRangeDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			FromValue = options.GetOrDefault(nameof(FromValue));
			ToValue = options.GetOrDefault(nameof(ToValue));
			DefaultFromValue = options.GetOrDefault(nameof(DefaultFromValue));
			DefaultToValue = options.GetOrDefault(nameof(DefaultToValue));
		}

		public SelectRangeDataFilterOptions(string propertyName, string inputType,
			bool isInverted = false,
			object fromValue = null, object toValue = null, object defaultFromValue = null, object defaultToValue = null,
			Guid? entityId = null, string enumType = null, List<DataFilterEnumValue> enumValues = null)
			: base(propertyName, isInverted, inputType: inputType, entityId: entityId, enumType: enumType, enumValues: enumValues)
		{
			FromValue = fromValue;
			ToValue = toValue;
			DefaultFromValue = defaultFromValue;
			DefaultToValue = defaultToValue;
		}

		public SelectRangeDataFilterOptions()
		{
		}

		public object FromValue { get; set; }
		public object ToValue { get; set; }
		public object DefaultFromValue { get; }
		public object DefaultToValue { get; }
	}

	public class SelectRangeDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SelectRangeDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var optionsWrapper = new SelectRangeDataFilterOptions(Converter, options);
			return PredicateBuilder.Between<TEntity>(optionsWrapper.PropertyName,
				optionsWrapper.FromValue ?? optionsWrapper.DefaultFromValue,
				optionsWrapper.ToValue ?? optionsWrapper.DefaultToValue);
		}
	}
}
