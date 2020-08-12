using System;
using Isap.CommonCore.Utils;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class ObjectToByteArrayConverter: IObjectConverter<object, byte[]>
	{
		public ConvertAttempt<byte[]> TryConvert(object sourceValue)
		{
			switch (sourceValue)
			{
				case null:
					return ConvertAttempt.Success<byte[]>(null);
				case bool value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case char value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case decimal _:
					throw new NotSupportedException();
				case double value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case float value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case int value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case long value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case short value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case uint value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case ulong value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case ushort value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value));
				case Guid value:
					return ConvertAttempt.Success(value.ToByteArray());
				case DateTime value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value.ToBinary()));
				case TimeSpan value:
					return ConvertAttempt.Success(BitConverter.GetBytes(value.Ticks));
			}

			if (sourceValue.GetType().IsValueType)
				return ConvertAttempt.Fail<byte[]>();
			return ConvertAttempt.Success(BsonHelpers.ToBson(sourceValue));
		}

		ConvertAttempt IObjectConverter.TryConvert(object sourceValue)
		{
			return TryConvert(sourceValue);
		}
	}
}
