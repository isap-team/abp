using System;

namespace Isap.Converters.BasicConverters
{
	public interface INullableValueTypeConverter<TTarget>: IBasicValueConverter
		where TTarget: struct
	{
		new ConvertAttempt<TTarget?> TryConvert(IBasicValueConverterProvider converterProvider, object value);
	}

	public class NullableValueTypeConverter<T>: INullableValueTypeConverter<T>
		where T: struct
	{
		public Type TargetType => typeof(T?);

		public ConvertAttempt<T?> TryConvert(IBasicValueConverterProvider converterProvider, object value)
		{
			if (value == null)
				return ConvertAttempt.Success((T?) null);
			IBasicValueConverter converter = converterProvider.GetBasicConverter(value.GetType(), typeof(T));
			ConvertAttempt attempt = converter.TryConvert(converterProvider, value);
			return attempt.IsSuccess
					? ConvertAttempt.Success((T?) attempt.Result)
					: ConvertAttempt.Fail<T?>()
				;
		}

		ConvertAttempt IBasicValueConverter.TryConvert(IBasicValueConverterProvider converterProvider, object value)
		{
			return TryConvert(converterProvider, value);
		}
	}

	public static class NullableValueTypeConverter
	{
		public static IBasicValueConverter Create(Type toType)
		{
			Type nullableUnderlyingType = toType.GetGenericArguments()[0];
			Type type = typeof(NullableValueTypeConverter<>).MakeGenericType(nullableUnderlyingType);
			return (IBasicValueConverter) Activator.CreateInstance(type);
		}
	}
}
