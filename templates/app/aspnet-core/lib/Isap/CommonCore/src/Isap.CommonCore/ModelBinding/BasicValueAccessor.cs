using System;
using Isap.Converters;

namespace Isap.CommonCore.ModelBinding
{
	public class BasicValueAccessor: IValueAccessor
	{
		private object _value;

		public BasicValueAccessor(IValueConverter converter, Type valueType, object value)
		{
			Converter = converter;
			ValueType = valueType;
			_value = value;
		}

		public IValueConverter Converter { get; }
		public Type ValueType { get; }

		public object GetValue()
		{
			return _value;
		}

		public void SetValue(object value)
		{
			_value = value;
		}

		public object EnsureValue()
		{
			if (_value == null)
				_value = Activator.CreateInstance(ValueType);
			return _value;
		}
	}
}
