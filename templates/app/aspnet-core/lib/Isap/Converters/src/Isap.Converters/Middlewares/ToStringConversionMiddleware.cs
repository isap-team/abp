using System;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;
using Newtonsoft.Json.Linq;

namespace Isap.Converters.Middlewares
{
	public class ToStringConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, IBasicValueConverter> __converterMap =
			new Dictionary<Type, IBasicValueConverter>
				{
					{ typeof(JToken), new SimpleValueConverter<JToken, string>(source => source.ToString()) },
				};

		public ToStringConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (toType == typeof(string))
			{
				if (__converterMap.TryGetValue(fromType, out IBasicValueConverter converter))
					return converter;
				Type converterType;
				if (fromType.IsValueType)
					converterType = typeof(ValueTypeToStringConverter<>).MakeGenericType(fromType);
				else
					converterType = typeof(ObjectToStringConverter<>).MakeGenericType(fromType);
				return (IBasicValueConverter) Activator.CreateInstance(converterType);
			}

			return Next.GetBasicConverter(fromType, toType);
		}
	}
}
