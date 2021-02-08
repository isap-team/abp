using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Converters.ValueTypeConverters
{
	public class ValueTypeToBooleanConverter: ValueTypeConverterBase<ValueType, bool>
	{
		private static readonly ConcurrentDictionary<Type, IValueTypeConverter<bool>> _converterMap =
			new ConcurrentDictionary<Type, IValueTypeConverter<bool>>(
				new Dictionary<Type, IValueTypeConverter<bool>>
					{
						{ typeof(bool), new BasicValueTypeConverter<bool, bool>(source => source) },
						//{ typeof(long), new ValueTypeParser<long>(long.TryParse) },
						//{ typeof(ulong), new ValueTypeParser<ulong>(ulong.TryParse) },
						//{ typeof(int), new ValueTypeParser<int>(int.TryParse) },
						//{ typeof(uint), new ValueTypeParser<uint>(uint.TryParse) },
						//{ typeof(short), new ValueTypeParser<short>(short.TryParse) },
						//{ typeof(ushort), new ValueTypeParser<ushort>(ushort.TryParse) },
						//{ typeof(sbyte), new ValueTypeParser<sbyte>(sbyte.TryParse) },
						//{ typeof(byte), new ValueTypeParser<byte>(byte.TryParse) },
						//{ typeof(decimal), new ValueTypeParser<decimal>(decimal.TryParse) },
						//{ typeof(float), new ValueTypeParser<float>(float.TryParse) },
						//{ typeof(double), new ValueTypeParser<double>(double.TryParse) },
						//{ typeof(Guid), new ValueTypeParser<Guid>(Guid.TryParse) },
						//{ typeof(DateTime), new ValueTypeParser<DateTime>(DateTime.TryParse) },
						//{ typeof(TimeSpan), new ValueTypeParser<TimeSpan>(TimeSpan.TryParse) },
					}
			);

		protected static ConvertAttempt<bool> Fail()
		{
			return ConvertAttempt.Fail<bool>();
		}

		public override ConvertAttempt<bool> TryConvert(ValueType sourceValue)
		{
			Type fromType = sourceValue.GetType();
			IValueTypeConverter<bool> converter;
			if (!_converterMap.TryGetValue(fromType, out converter))
				return Fail();
			return converter.TryConvert(sourceValue);
		}
	}
}
