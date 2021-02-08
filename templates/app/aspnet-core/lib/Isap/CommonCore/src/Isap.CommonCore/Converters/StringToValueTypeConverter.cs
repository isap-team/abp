using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Isap.CommonCore.Converters.ValueTypeConverters;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public interface IValueConverter<in TValue>
	{
		ConvertAttempt TryConvertTo(TValue value, Type toType);
		ConvertAttempt<TResult> TryConvertTo<TResult>(TValue value);
	}

	public class StringToValueTypeConverter: ValueConverterBase<string>
	{
		private static readonly ConcurrentDictionary<Type, IValueTypeConverter> _converterMap =
			new ConcurrentDictionary<Type, IValueTypeConverter>(
				new Dictionary<Type, IValueTypeConverter>
					{
						{ typeof(bool), new StringToBooleanConverter() },
						{ typeof(long), new ValueTypeParser<long>(long.TryParse) },
						{ typeof(ulong), new ValueTypeParser<ulong>(ulong.TryParse) },
						{ typeof(int), new ValueTypeParser<int>(int.TryParse) },
						{ typeof(uint), new ValueTypeParser<uint>(uint.TryParse) },
						{ typeof(short), new ValueTypeParser<short>(short.TryParse) },
						{ typeof(ushort), new ValueTypeParser<ushort>(ushort.TryParse) },
						{ typeof(sbyte), new ValueTypeParser<sbyte>(sbyte.TryParse) },
						{ typeof(byte), new ValueTypeParser<byte>(byte.TryParse) },
						{ typeof(decimal), new ValueTypeParser<decimal>(decimal.TryParse) },
						{ typeof(float), new ValueTypeParser<float>(float.TryParse) },
						{ typeof(double), new ValueTypeParser<double>(double.TryParse) },
						{ typeof(Guid), new ValueTypeParser<Guid>(Guid.TryParse) },
						{ typeof(DateTime), new ValueTypeParser<DateTime>(DateTime.TryParse) },
						{ typeof(TimeSpan), new ValueTypeParser<TimeSpan>(TimeSpan.TryParse) },
					}
			);

		public override ConvertAttempt TryConvertTo(string value, Type toType)
		{
			if (!_converterMap.TryGetValue(toType, out IValueTypeConverter converter))
			{
				TypeInfo typeInfo = toType.GetTypeInfo();
				if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
					converter = _converterMap.GetOrAdd(toType, NullableValueTypeConverter.Create);
				else if (typeInfo.IsEnum)
					converter = _converterMap.GetOrAdd(toType, EnumParser.Create);
				else
					return ConvertAttempt.Fail(toType);
			}

			return converter.TryConvert(value);
		}
	}
}
