namespace Isap.Converters.BasicConverters
{
	public class SameTypeValueConverter<T>: BasicValueConverterBase<T, T>
	{
		public override ConvertAttempt<T> TryConvert(IBasicValueConverterProvider converterProvider, T value)
		{
			return ConvertAttempt.Success(value);
		}
	}
}
