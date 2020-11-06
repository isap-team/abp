using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public interface INullableValueTypeConverter<TTarget>: IValueTypeConverter
		where TTarget: struct
	{
		new ConvertAttempt<TTarget?> TryConvert(object sourceValue);
	}
}
