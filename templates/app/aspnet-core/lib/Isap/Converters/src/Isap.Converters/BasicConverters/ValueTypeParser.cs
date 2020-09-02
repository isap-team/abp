namespace Isap.Converters.BasicConverters
{
	public delegate bool TryParseDelegate<T>(string value, out T result);

	public class ValueTypeParser<T>: BasicValueConverterBase<string, T>
		where T: struct
	{
		private readonly TryParseDelegate<T> _tryParse;

		public ValueTypeParser(TryParseDelegate<T> tryParse)
		{
			_tryParse = tryParse;
		}

		public override ConvertAttempt<T> TryConvert(IBasicValueConverterProvider converterProvider, string value)
		{
			return _tryParse(value, out T result)
					? ConvertAttempt.Success(result)
					: ConvertAttempt.Fail<T>()
				;
		}
	}
}
