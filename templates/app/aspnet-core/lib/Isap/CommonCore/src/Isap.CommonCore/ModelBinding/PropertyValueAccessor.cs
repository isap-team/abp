using System;
using System.ComponentModel;
using Isap.Converters;

namespace Isap.CommonCore.ModelBinding
{
	public class PropertyValueAccessor: IValueAccessor
	{
		private readonly IValueAccessor _componentValueAccessor;
		private readonly PropertyDescriptor _property;

		public PropertyValueAccessor(IValueAccessor componentValueAccessor, PropertyDescriptor property)
		{
			_componentValueAccessor = componentValueAccessor;
			_property = property;
		}

		public IValueConverter Converter => _componentValueAccessor.Converter;
		public Type ValueType => _property.PropertyType;

		public object GetValue()
		{
			object component = _componentValueAccessor.GetValue();
			return component == null ? null : _property.GetValue(component);
		}

		public void SetValue(object value)
		{
			value = Converter.ConvertTo(ValueType, value);
			_property.SetValue(_componentValueAccessor.EnsureValue(), value);
		}

		public object EnsureValue()
		{
			object result = GetValue();
			if (result == null)
			{
				result = Activator.CreateInstance(ValueType);
				SetValue(result);
			}

			return result;
		}
	}
}
