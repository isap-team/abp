using System;
using Isap.Converters;

namespace Isap.CommonCore
{
	public class ProvidedValuesValidator
	{
		public ProvidedValuesValidator()
		{
			Converter = ValueConverterProviders.Default.GetConverter();
		}

		public IValueConverter Converter { get; set; }

		/// <summary>
		///     Состояние проверки. Вернет положительный результат при наличии значений в проверяемых типах.
		/// </summary>
		public bool IsAnyValueProvided { get; private set; }

		public object CheckIsProvided(Type type, object value)
		{
			while (true)
			{
				if (value == null)
					break;

				if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					IsAnyValueProvided = true;
					break;
				}

				switch (value)
				{
					case string strValue:
						IsAnyValueProvided = IsAnyValueProvided || !string.IsNullOrEmpty(strValue) && Converter.TryConvertTo(type, strValue).IsSuccess;
						break;
					default:
						IsAnyValueProvided = true;
						break;
				}

				break;
			}

			return value;
		}

		public T CheckIsProvided<T>(T value)
		{
			return (T) CheckIsProvided(typeof(T), value);
		}
	}
}
