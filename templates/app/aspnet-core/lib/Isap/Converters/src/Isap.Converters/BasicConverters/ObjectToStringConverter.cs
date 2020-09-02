namespace Isap.Converters.BasicConverters
{
	public class ObjectToStringConverter<T>: BasicValueConverterBase<T, string>
		where T: class
	{
		public override ConvertAttempt<string> TryConvert(IBasicValueConverterProvider converterProvider, T value)
		{
			return value == null
					? ConvertAttempt.Success((string) null)
					: ConvertAttempt.Success(value.ToString())
				;
		}
	}
}
