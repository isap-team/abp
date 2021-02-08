using System;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;

namespace Isap.Converters.Middlewares
{
	public class StringToValueTypeConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, IBasicValueConverter> __converterMap =
			new Dictionary<Type, IBasicValueConverter>
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
					{ typeof(Uri), new StringToUriConverter() },
				};

		public StringToValueTypeConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (fromType == typeof(string))
			{
				if (__converterMap.TryGetValue(toType, out IBasicValueConverter converter))
					return converter;
				if (toType.IsEnum)
				{
					return EnumParser.Create(toType);
				}
			}

			return Next.GetBasicConverter(fromType, toType);
		}
	}
}
