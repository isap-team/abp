namespace Isap.Converters.BasicConverters
{
	public class ValueTypeToStringConverter<T>: BasicValueConverterBase<T, string>
		where T: struct
	{
		public override ConvertAttempt<string> TryConvert(IBasicValueConverterProvider converterProvider, T value)
		{
			return ConvertAttempt.Success(value.ToString());
		}
	}
}
