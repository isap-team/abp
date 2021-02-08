using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class ByteArrayToValueConverter: ValueConverterBase<byte[]>
	{
		private static readonly ConcurrentDictionary<Type, IValueTypeConverter> _converterMap =
			new ConcurrentDictionary<Type, IValueTypeConverter>(
				new Dictionary<Type, IValueTypeConverter>
					{
						{ typeof(bool), new ValueTypeBitConverter<bool>(BitConverter.ToBoolean) },
						{ typeof(long), new ValueTypeBitConverter<long>(BitConverter.ToInt64) },
						{ typeof(ulong), new ValueTypeBitConverter<ulong>(BitConverter.ToUInt64) },
						{ typeof(int), new ValueTypeBitConverter<int>(BitConverter.ToInt32) },
						{ typeof(uint), new ValueTypeBitConverter<uint>(BitConverter.ToUInt32) },
						{ typeof(short), new ValueTypeBitConverter<short>(BitConverter.ToInt16) },
						{ typeof(ushort), new ValueTypeBitConverter<ushort>(BitConverter.ToUInt16) },
						{ typeof(sbyte), new ValueTypeBitConverter<sbyte>((bytes, i) => (sbyte) bytes[i]) },
						{ typeof(byte), new ValueTypeBitConverter<byte>((bytes, i) => bytes[i]) },

						//{ typeof(decimal), new ValueTypeBitConverter<decimal>(BitConverter) },
						{ typeof(float), new ValueTypeBitConverter<float>(BitConverter.ToSingle) },
						{ typeof(double), new ValueTypeBitConverter<double>(BitConverter.ToDouble) },

						{ typeof(Guid), new ValueTypeBitConverter<Guid>((bytes, i) => new Guid(bytes)) },
						{ typeof(DateTime), new ValueTypeBitConverter<DateTime>((bytes, i) => DateTime.FromBinary(BitConverter.ToInt64(bytes, i))) },
						{ typeof(TimeSpan), new ValueTypeBitConverter<TimeSpan>((bytes, i) => TimeSpan.FromTicks(BitConverter.ToInt64(bytes, i))) },
					}
			);

		public override ConvertAttempt TryConvertTo(byte[] value, Type toType)
		{
			IValueTypeConverter converter;
			if (_converterMap.TryGetValue(toType, out converter))
				return converter.TryConvert(value);
			return ConvertAttempt.Fail(toType);
		}
	}
}
