using System;
using Isap.Converters;

namespace Isap.CommonCore.ModelBinding
{
	public interface IValueAccessor
	{
		IValueConverter Converter { get; }
		Type ValueType { get; }

		object GetValue();
		void SetValue(object value);
		object EnsureValue();
	}
}
