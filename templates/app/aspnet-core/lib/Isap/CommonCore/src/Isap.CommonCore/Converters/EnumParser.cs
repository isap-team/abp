using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public static class EnumParser
	{
		public static IValueTypeConverter Create(Type enumType)
		{
			Type type = typeof(EnumParser<>).MakeGenericType(enumType);
			return (IValueTypeConverter) Activator.CreateInstance(type);
		}
	}

	public class EnumParser<TEnum>: ValueTypeConverterBase<string, TEnum>
		where TEnum: struct
	{
		public override ConvertAttempt<TEnum> TryConvert(string sourceValue)
		{
			return Enum.TryParse(sourceValue, true, out TEnum result) ? ConvertAttempt.Success(result) : ConvertAttempt.Fail<TEnum>();
		}
	}
}
