using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public abstract class ValueTypeConverterBase<TSource, TTarget>: IValueTypeConverter<TSource, TTarget>
		where TTarget: struct
	{
		public abstract ConvertAttempt<TTarget> TryConvert(TSource sourceValue);

		ConvertAttempt<TTarget> IValueTypeConverter<TTarget>.TryConvert(object sourceValue)
		{
			return TryConvert((TSource) sourceValue);
		}

		ConvertAttempt IValueTypeConverter.TryConvert(object sourceValue)
		{
			return TryConvert((TSource) sourceValue);
		}
	}
}
