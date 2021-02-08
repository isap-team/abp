using System;

namespace Isap.Converters
{
	public interface IValueConverter
	{
		object ConvertTo(Type toType, object value);
		T ConvertTo<T>(object value);

		ConvertAttempt TryConvertTo(Type toType, object value);
		ConvertAttempt<T> TryConvertTo<T>(object value);
	}
}
