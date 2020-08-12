using System;
using Isap.Converters.BasicConverters;

namespace Isap.Converters
{
	public class FailoverValueConversionMiddleware: ValueConversionMiddlewareBase
	{
		public FailoverValueConversionMiddleware()
			: base(null)
		{
		}

		protected override IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType)
		{
			return fromType == toType
					? (IBasicValueConverter) Activator.CreateInstance(typeof(SameTypeValueConverter<>).MakeGenericType(toType))
					: new FailValueConverter(toType)
				;
		}
	}
}
