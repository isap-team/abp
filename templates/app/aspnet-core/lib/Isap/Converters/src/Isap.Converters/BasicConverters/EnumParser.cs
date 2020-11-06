using System;

namespace Isap.Converters.BasicConverters
{
	public static class EnumParser
	{
		public static IBasicValueConverter Create(Type enumType)
		{
			Type type = typeof(EnumParser<>).MakeGenericType(enumType);
			return (IBasicValueConverter) Activator.CreateInstance(type);
		}
	}

	public class EnumParser<TEnum>: BasicValueConverterBase<string, TEnum>
		where TEnum: struct
	{
		public override ConvertAttempt<TEnum> TryConvert(IBasicValueConverterProvider converterProvider, string value)
		{
			TEnum result;
			return Enum.TryParse(value, true, out result)
					? ConvertAttempt.Success(result)
					: ConvertAttempt.Fail<TEnum>()
				;
		}
	}
}
