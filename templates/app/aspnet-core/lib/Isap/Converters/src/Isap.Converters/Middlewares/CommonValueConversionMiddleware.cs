using System;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;

namespace Isap.Converters.Middlewares
{
	public class CommonValueConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, Dictionary<Type, IBasicValueConverter>> _converterMap =
			new Dictionary<Type, Dictionary<Type, IBasicValueConverter>>
				{
					{
						typeof(long),
						new Dictionary<Type, IBasicValueConverter>
							{
								{ typeof(int), new SimpleValueConverter<long, int>(value => (int) value) },
								{ typeof(decimal), new SimpleValueConverter<long, decimal>(value => (decimal) value) },
							}
					},
				};

		public CommonValueConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (toType.IsAssignableFrom(fromType))
				return (IBasicValueConverter) Activator.CreateInstance(typeof(SameTypeValueConverter<>).MakeGenericType(toType));
			if (_converterMap.TryGetValue(fromType, out Dictionary<Type, IBasicValueConverter> converterMap))
			{
				if (converterMap.TryGetValue(toType, out IBasicValueConverter converter))
					return converter;
			}

			return null;
		}
	}
}
