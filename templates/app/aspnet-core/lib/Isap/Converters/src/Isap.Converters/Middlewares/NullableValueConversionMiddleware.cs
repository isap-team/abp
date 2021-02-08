using System;
using Isap.Converters.BasicConverters;

namespace Isap.Converters.Middlewares
{
	public class NullableValueConversionMiddleware: ValueConversionMiddlewareBase
	{
		public NullableValueConversionMiddleware(IValueConversionMiddleware next)
			: base(next)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return NullableValueTypeConverter.Create(toType);
			return Next.GetBasicConverter(fromType, toType);
		}
	}
}
