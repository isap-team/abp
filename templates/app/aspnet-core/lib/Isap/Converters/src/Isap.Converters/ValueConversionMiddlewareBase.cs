using System;

namespace Isap.Converters
{
	public abstract class ValueConversionMiddlewareBase: IValueConversionMiddleware
	{
		protected ValueConversionMiddlewareBase(IValueConversionMiddleware next)
		{
			Next = next;
		}

		public IValueConversionMiddleware Next { get; }

		public virtual IBasicValueConverter GetBasicConverter(Type fromType, Type toType)
		{
			IBasicValueConverter converter = InternalGetBasicConverter(fromType, toType);
			if (converter == null && Next != null)
				converter = Next.GetBasicConverter(fromType, toType);
			return converter;
		}

		protected abstract IBasicValueConverter InternalGetBasicConverter(Type fromType, Type toType);
	}
}
