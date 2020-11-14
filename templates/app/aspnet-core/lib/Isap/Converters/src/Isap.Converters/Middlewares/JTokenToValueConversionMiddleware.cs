using System;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;
using Newtonsoft.Json.Linq;

namespace Isap.Converters.Middlewares
{
	public class JTokenToValueConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, IBasicValueConverter> _converterMap =
			new Dictionary<Type, IBasicValueConverter>
				{
					{ typeof(bool), new JValueConverter<bool>() },
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
					{ typeof(Guid), new JValueConverter<Guid>() },
					//{ typeof(DateTime), new ValueTypeParser<DateTime>(DateTime.TryParse) },
					//{ typeof(TimeSpan), new ValueTypeParser<TimeSpan>(TimeSpan.TryParse) },
				};

		public JTokenToValueConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (fromType.IsAssignableFrom(typeof(JValue)))
			{
				if (_converterMap.TryGetValue(toType, out IBasicValueConverter converter))
					return converter;
			}
			else if (fromType.IsAssignableFrom(typeof(JToken)))
			{
				throw new NotImplementedException();
			}

			return Next.GetBasicConverter(fromType, toType);
		}
	}
}
