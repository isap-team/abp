using System;

namespace Isap.Converters
{
	public interface IBasicValueConverter
	{
		Type TargetType { get; }
		ConvertAttempt TryConvert(IBasicValueConverterProvider converterProvider, object value);
	}

	public interface IBasicValueConverter<TTarget>: IBasicValueConverter
	{
		new ConvertAttempt<TTarget> TryConvert(IBasicValueConverterProvider converterProvider, object value);
	}

	public interface IBasicValueConverter<in TSource, TTarget>: IBasicValueConverter<TTarget>
	{
		ConvertAttempt<TTarget> TryConvert(IBasicValueConverterProvider converterProvider, TSource value);
	}
}
