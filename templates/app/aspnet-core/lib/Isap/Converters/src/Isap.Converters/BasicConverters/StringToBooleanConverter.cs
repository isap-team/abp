namespace Isap.Converters.BasicConverters
{
	public class StringToBooleanConverter: BasicValueConverterBase<string, bool>
	{
		public override ConvertAttempt<bool> TryConvert(IBasicValueConverterProvider converterProvider, string value)
		{
			bool result;
			if (bool.TryParse(value, out result))
				return ConvertAttempt.Success(result);
			ConvertAttempt<int> attempt = new ValueTypeParser<int>(int.TryParse).TryConvert(converterProvider, value);
			if (attempt.IsSuccess)
				return ConvertAttempt.Success(attempt.Result != 0);
			return ConvertAttempt.Fail<bool>();
		}
	}
}
