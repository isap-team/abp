using System;
using System.Collections;
using System.Collections.Generic;
using Isap.Converters.BasicConverters;

namespace Isap.Converters.Middlewares
{
	public class EnumerableValueConversionMiddleware: ValueConversionMiddlewareBase
	{
		private static readonly Dictionary<Type, Func<Type, IBasicValueConverter>> __converterMap =
			new Dictionary<Type, Func<Type, IBasicValueConverter>>
				{
					{ typeof(List<>), EnumerableToListConverter.Create },
					{ typeof(IList<>), EnumerableToListConverter.Create },
				};

		public EnumerableValueConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (fromType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(fromType))
			{
				if (toType.IsArray)
					return EnumerableToArrayConverter.Create(toType.GetElementType());
				if (toType.IsGenericType)
				{
					if (__converterMap.TryGetValue(toType.GetGenericTypeDefinition(), out Func<Type, IBasicValueConverter> ctor))
						return ctor(toType.GetGenericArguments()[0]);
				}
			}

			return null;
		}
	}
}
