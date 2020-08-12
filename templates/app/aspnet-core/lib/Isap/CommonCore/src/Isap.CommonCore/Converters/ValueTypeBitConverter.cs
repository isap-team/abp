using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class ValueTypeBitConverter<T>: ValueTypeConverterBase<byte[], T>
		where T: struct
	{
		private readonly Func<byte[], int, T> _convert;

		public ValueTypeBitConverter(Func<byte[], int, T> convert)
		{
			_convert = convert;
		}

		public override ConvertAttempt<T> TryConvert(byte[] sourceValue)
		{
			return ConvertAttempt.Success(_convert(sourceValue, 0));
		}
	}
}
