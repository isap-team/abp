using Isap.Converters;

namespace Isap.CommonCore.Converters.ValueTypeConverters
{
	public class StringToBooleanConverter: ValueTypeConverterBase<string, bool>
	{
		public override ConvertAttempt<bool> TryConvert(string sourceValue)
		{
			if (bool.TryParse(sourceValue, out bool result))
				return ConvertAttempt.Success(result);
			ConvertAttempt<int> attempt = new ValueTypeParser<int>(int.TryParse).TryConvert(sourceValue);
			if (attempt.IsSuccess)
				return ConvertAttempt.Success(attempt.Result != 0);
			return ConvertAttempt.Fail<bool>();
		}
	}
}
