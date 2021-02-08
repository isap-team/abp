using System;

namespace Isap.Converters.BasicConverters
{
	public class StringToUriConverter: BasicValueConverterBase<string, Uri>
	{
		public override ConvertAttempt<Uri> TryConvert(IBasicValueConverterProvider converterProvider, string value)
		{
			try
			{
				return ConvertAttempt.Success(new Uri(value));
			}
			catch (Exception)
			{
				return ConvertAttempt.Fail<Uri>();
			}
		}
	}
}
