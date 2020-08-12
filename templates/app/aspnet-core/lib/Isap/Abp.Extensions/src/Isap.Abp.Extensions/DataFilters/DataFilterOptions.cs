using System;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters
{
	public class DataFilterOptions
	{
		public DataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
		{
			PropertyName = converter.ConvertTo<string>(options.GetOrDefault(nameof(PropertyName)));
			IsInverted = converter.ConvertTo<bool>(options.GetOrDefault(nameof(IsInverted)));
			InputType = converter.ConvertTo<string>(options.GetOrDefault(nameof(InputType)));
			EntityId = converter.TryConvertTo<Guid?>(options.GetOrDefault(nameof(EntityId))).AsDefaultIfNotSuccess();
			EnumType = converter.ConvertTo<string>(options.GetOrDefault(nameof(EnumType)));
			EnumValues = converter.ConvertTo<List<DataFilterEnumValue>>(options.GetOrDefault(nameof(EnumValues)));
			IsIgnoreSafeDeleteFilter = converter.ConvertTo<bool>(options.GetOrDefault(nameof(IsIgnoreSafeDeleteFilter)));
			IsIgnoreMultiTenantFilter = converter.ConvertTo<bool>(options.GetOrDefault(nameof(IsIgnoreMultiTenantFilter)));
			SearchValueConverterName = converter.ConvertTo<string>(options.GetOrDefault(nameof(SearchValueConverterName)));
			CustomPredicateBuilderName = converter.ConvertTo<string>(options.GetOrDefault(nameof(CustomPredicateBuilderName)));
		}

		public DataFilterOptions(string propertyName, bool isInverted, string inputType = null, Guid? entityId = null, string enumType = null,
			List<DataFilterEnumValue> enumValues = null, string searchValueConverterName = null, string customPredicateBuilderName = null)
		{
			PropertyName = propertyName;
			IsInverted = isInverted;
			InputType = inputType;
			EntityId = entityId;
			EnumType = enumType;
			EnumValues = enumValues;
			SearchValueConverterName = searchValueConverterName;
			CustomPredicateBuilderName = customPredicateBuilderName;
		}

		public DataFilterOptions()
		{
		}

		public string PropertyName { get; set; }

		public bool IsInverted { get; set; }

		public string InputType { get; set; }
		public Guid? EntityId { get; set; }
		public string EnumType { get; set; }
		public List<DataFilterEnumValue> EnumValues { get; set; }

		public bool IsIgnoreSafeDeleteFilter { get; set; }
		public bool IsIgnoreMultiTenantFilter { get; set; }

		public string SearchValueConverterName { get; set; }
		public string CustomPredicateBuilderName { get; set; }
	}
}
