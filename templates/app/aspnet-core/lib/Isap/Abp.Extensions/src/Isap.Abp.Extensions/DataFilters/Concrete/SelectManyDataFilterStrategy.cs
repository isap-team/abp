using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Isap.Converters;

namespace Isap.Abp.Extensions.DataFilters.Concrete
{
	public class SelectManyDataFilterOptions: DataFilterOptions
	{
		public SelectManyDataFilterOptions(IValueConverter converter, Dictionary<string, object> options)
			: base(converter, options)
		{
			Values = (ICollection) options.GetOrDefault(nameof(Values));
		}

		public SelectManyDataFilterOptions(string propertyName, string inputType = null, object values = null, Guid? entityId = null,
			string enumType = null, List<DataFilterEnumValue> enumValues = null, bool isInverted = false)
			: base(propertyName, isInverted, inputType: inputType, entityId: entityId, enumType: enumType, enumValues: enumValues)
		{
			Values = (ICollection) values;
		}

		public SelectManyDataFilterOptions()
		{
		}

		public ICollection Values { get; set; }
	}

	public class SelectManyDataFilterStrategy<TEntity>: DataFilterStrategyBase<TEntity>
	{
		public SelectManyDataFilterStrategy(Dictionary<string, object> options)
			: base(options)
		{
		}

		protected override Expression<Func<TEntity, bool>> CreateExpression(Dictionary<string, object> options)
		{
			var optionsWrapper = new SelectManyDataFilterOptions(Converter, options);
			if (optionsWrapper.Values == null)
				return PredicateBuilder.True<TEntity>();
			switch (optionsWrapper.Values.Count)
			{
				case 0:
					return PredicateBuilder.True<TEntity>();
				case 1:
					object value = optionsWrapper.Values.Cast<object>().First();
					//Enable next lines when CaseSensitive == true
					//if (IsString(value, out string searchExpr))
					//	return PredicateBuilder.ILike<TEntity>(optionsWrapper.PropertyName, searchExpr);
					return PredicateBuilder.Equal<TEntity>(optionsWrapper.PropertyName, value);
				default:
					return PredicateBuilder.InCollection<TEntity>(optionsWrapper.PropertyName, optionsWrapper.Values);
			}
		}

		public override Dictionary<string, object> CompleteOptions(Dictionary<string, object> options)
		{
			options = base.CompleteOptions(options);
			var filterOptions = new SelectManyDataFilterOptions(Converter, options);
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
