using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public interface IValueTypeConverter
	{
		ConvertAttempt TryConvert(object sourceValue);
	}

	public interface IValueTypeConverter<TTarget>: IValueTypeConverter
		where TTarget: struct
	{
		new ConvertAttempt<TTarget> TryConvert(object sourceValue);
	}

	public interface IValueTypeConverter<in TSource, TTarget>: IValueTypeConverter<TTarget>
		where TTarget: struct
	{
		ConvertAttempt<TTarget> TryConvert(TSource sourceValue);
	}
}
