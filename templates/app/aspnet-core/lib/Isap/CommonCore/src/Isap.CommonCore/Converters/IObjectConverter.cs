using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public interface IObjectConverter
	{
		ConvertAttempt TryConvert(object sourceValue);
	}

	public interface IObjectConverter<in TSource, TTarget>: IObjectConverter
		where TTarget: class
	{
		ConvertAttempt<TTarget> TryConvert(TSource sourceValue);
	}
}
