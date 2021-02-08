using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Converters.ValueTypeConverters
{
	public class ValueTypeToInt32Converter: ValueTypeConverterBase<ValueType, int>
	{
		private static readonly ConcurrentDictionary<Type, IValueTypeConverter<int>> _converterMap =
			new ConcurrentDictionary<Type, IValueTypeConverter<int>>(
				new Dictionary<Type, IValueTypeConverter<int>>
					{
						//{ typeof(bool), new BasicValueTypeConverter<bool, bool>(source => source) },
						{ typeof(long), new BasicValueTypeConverter<long, int>(source => (int) source) },
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

		protected static ConvertAttempt<int> Fail()
		{
			return ConvertAttempt.Fail<int>();
		}

		public override ConvertAttempt<int> TryConvert(ValueType sourceValue)
		{
			Type fromType = sourceValue.GetType();
			if (!_converterMap.TryGetValue(fromType, out IValueTypeConverter<int> converter))
				return Fail();
			return converter.TryConvert(sourceValue);
		}
	}
}
