using System;

namespace Isap.Converters
{
	public abstract class BasicValueConverterBase<TSource, TTarget>: IBasicValueConverter<TSource, TTarget>
	{
		public Type TargetType => typeof(TTarget);

		public abstract ConvertAttempt<TTarget> TryConvert(IBasicValueConverterProvider converterProvider, TSource value);

		ConvertAttempt<TTarget> IBasicValueConverter<TTarget>.TryConvert(IBasicValueConverterProvider converterProvider, object value)
		{
			return TryConvert(converterProvider, (TSource) value);
		}

		ConvertAttempt IBasicValueConverter.TryConvert(IBasicValueConverterProvider converterProvider, object value)
		{
			return TryConvert(converterProvider, (TSource) value);
		}
	}
}
