using System;
using Newtonsoft.Json.Linq;

namespace Isap.Converters.BasicConverters
{
	public static class JValueToEnumConverter
	{
		public static IBasicValueConverter Create(Type enumType)
		{
			return (IBasicValueConverter) Activator.CreateInstance(typeof(JValueToEnumConverter<>).MakeGenericType(enumType));
		}
	}

	public class JValueToEnumConverter<TEnum>: BasicValueConverterBase<JValue, TEnum>
	{
		public override ConvertAttempt<TEnum> TryConvert(IBasicValueConverterProvider converterProvider, JValue value)
		{
			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
			IBasicValueConverter converter = GetUnderlyingConverter(converterProvider, value, underlyingType);
			ConvertAttempt attempt = converter.TryConvert(converterProvider, value.Value);
			if (!attempt.IsSuccess)
				return ConvertAttempt.Fail<TEnum>();
			return ConvertAttempt.Success((TEnum) Enum.ToObject(typeof(TEnum), attempt.Result));
		}

		private static IBasicValueConverter GetUnderlyingConverter(IBasicValueConverterProvider converterProvider, JValue value, Type enumUnderlyingType)
		{
			switch (value.Type)
			{
				case JTokenType.Integer:
					return converterProvider.GetBasicConverter(value.Value.GetType(), enumUnderlyingType);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
