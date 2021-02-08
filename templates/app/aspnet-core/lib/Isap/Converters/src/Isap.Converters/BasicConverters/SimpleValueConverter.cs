using System;

namespace Isap.Converters.BasicConverters
{
	public class SimpleValueConverter<TSource, TTarget>: BasicValueConverterBase<TSource, TTarget>
	{
		private readonly Func<TSource, TTarget> _convert;

		public SimpleValueConverter(Func<TSource, TTarget> convert)
		{
			_convert = convert;
		}

		public override ConvertAttempt<TTarget> TryConvert(IBasicValueConverterProvider converterProvider, TSource value)
		{
			return ConvertAttempt.Success(_convert(value));
		}
	}
}
