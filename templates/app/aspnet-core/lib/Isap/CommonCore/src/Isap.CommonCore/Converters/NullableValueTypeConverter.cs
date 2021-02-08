using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public static class NullableValueTypeConverter
	{
		public static IValueTypeConverter Create(Type toType)
		{
			Type nullableUnderlyingType = toType.GetGenericArguments()[0];
			Type type = typeof(NullableValueTypeConverter<>).MakeGenericType(nullableUnderlyingType);
			return (IValueTypeConverter) Activator.CreateInstance(type);
		}
	}

	public class NullableValueTypeConverter<T>: INullableValueTypeConverter<T>
		where T: struct
	{
		public ConvertAttempt<T?> TryConvert(object sourceValue)
		{
			if (sourceValue == null)
				ConvertAttempt.Success((T?) null);
			ConvertAttempt<T> attempt = ConvertExtensionsTemp.TryConvertTo<T>(sourceValue);
			return attempt.IsSuccess
					? ConvertAttempt.Success((T?) attempt.Result)
					: ConvertAttempt.Fail<T?>()
				;
		}

		ConvertAttempt IValueTypeConverter.TryConvert(object sourceValue)
		{
			return TryConvert((string) sourceValue);
		}
	}
}
