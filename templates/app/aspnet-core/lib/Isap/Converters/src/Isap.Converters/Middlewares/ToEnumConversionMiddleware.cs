using System;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;
using Newtonsoft.Json.Linq;

namespace Isap.Converters.Middlewares
{
	public class ToEnumConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, Func<Type, IBasicValueConverter>> __converterMap =
			new Dictionary<Type, Func<Type, IBasicValueConverter>>
				{
					{ typeof(JValue), JValueToEnumConverter.Create },
				};

		public ToEnumConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (toType.IsEnum)
			{
				if (__converterMap.TryGetValue(fromType, out var ctor))
					return ctor(toType);
				throw new NotImplementedException();
			}

			return Next.GetBasicConverter(fromType, toType);
		}
	}
}
