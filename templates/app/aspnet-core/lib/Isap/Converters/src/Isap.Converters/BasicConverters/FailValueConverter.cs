using System;

namespace Isap.Converters.BasicConverters
{
	public class FailValueConverter: IBasicValueConverter
	{
		public FailValueConverter(Type targetType)
		{
			TargetType = targetType;
		}

		public Type TargetType { get; }

		public ConvertAttempt TryConvert(IBasicValueConverterProvider converterProvider, object value)
		{
			return ConvertAttempt.Fail(TargetType);
		}
	}
}
