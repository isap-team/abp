using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class DefaultValueTypeConverter<TSource, TTarget>: ValueTypeConverterBase<TSource, TTarget>
		where TTarget: struct
	{
		private readonly Func<TSource, ConvertAttempt<TTarget>> _convert;

		public DefaultValueTypeConverter(Func<TSource, ConvertAttempt<TTarget>> convert)
		{
			_convert = convert;
		}

		public override ConvertAttempt<TTarget> TryConvert(TSource sourceValue)
		{
			return _convert(sourceValue);
		}
	}
}
