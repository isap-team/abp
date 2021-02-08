using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public abstract class ValueConverterBase<TValue>: IValueConverter<TValue>
	{
		public abstract ConvertAttempt TryConvertTo(TValue value, Type toType);

		public ConvertAttempt<TResult> TryConvertTo<TResult>(TValue value)
		{
			return TryConvertTo(value, typeof(TResult)).Cast<TResult>();
		}
	}
}
