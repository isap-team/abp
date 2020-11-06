using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class ValueTypeParser<T>: ValueTypeConverterBase<string, T>
		where T: struct
	{
		private readonly TryParseDelegate<T> _tryParse;

		public ValueTypeParser(TryParseDelegate<T> tryParse)
		{
			_tryParse = tryParse;
		}

		public override ConvertAttempt<T> TryConvert(string sourceValue)
		{
			return _tryParse(sourceValue, out T result) ? ConvertAttempt.Success(result) : ConvertAttempt.Fail<T>();
		}
	}
}
