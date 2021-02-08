using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class ObjectToStringConverter: IObjectConverter<object, string>
	{
		public ConvertAttempt<string> TryConvert(object sourceValue)
		{
			return sourceValue == null ? ConvertAttempt.Fail<string>() : ConvertAttempt.Success(Convert.ToString(sourceValue));
		}

		ConvertAttempt IObjectConverter.TryConvert(object sourceValue)
		{
			return TryConvert(sourceValue);
		}
	}
}
