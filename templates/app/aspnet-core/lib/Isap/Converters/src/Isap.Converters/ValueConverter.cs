using System;
using Isap.Converters.BasicConverters;
using Isap.Converters.Extensions;

namespace Isap.Converters
{
	public class ValueConverter: IValueConverter
	{
		protected class DefaultValueConverter: IBasicValueConverter
		{
			public DefaultValueConverter(Type targetType)
			{
				TargetType = targetType;
			}

			public Type TargetType { get; }

			public ConvertAttempt TryConvert(IBasicValueConverterProvider converterProvider, object value)
			{
				return ConvertAttempt.Success(TargetType.GetDefaultValue());
			}
		}

		public ValueConverter(IBasicValueConverterProvider provider)
		{
			Provider = provider;
		}

		public IBasicValueConverterProvider Provider { get; }

		public object ConvertTo(Type toType, object value)
		{
			ConvertAttempt attempt = TryConvertTo(toType, value);
			if (!attempt.IsSuccess)
			{
				if (value == null)
					throw new InvalidOperationException($"There is no possible conversion from null value to type '{toType}'.");
				throw new InvalidOperationException($"There is no possible conversion from value '{value}' with type '{value.GetType()}' to type '{toType}'.");
			}

			return attempt.Result;
		}

		public T ConvertTo<T>(object value)
		{
			ConvertAttempt<T> attempt = TryConvertTo<T>(value);
			if (!attempt.IsSuccess)
			{
				if (value == null)
					throw new InvalidOperationException($"There is no possible conversion from null value to type '{typeof(T)}'.");
				throw new InvalidOperationException(
					$"There is no possible conversion from value '{value}' with type '{value.GetType()}' to type '{typeof(T)}'.");
			}

			return attempt.Result;
		}

		public virtual ConvertAttempt TryConvertTo(Type toType, object value)
		{
			IBasicValueConverter converter = GetConverterFor(value, toType);
			return converter.TryConvert(Provider, value);
		}

		public virtual ConvertAttempt<T> TryConvertTo<T>(object value)
		{
			Type toType = typeof(T);
			IBasicValueConverter converter = GetConverterFor(value, toType);
			return converter is IBasicValueConverter<T> typedConverter
					? typedConverter.TryConvert(Provider, value)
					: converter.TryConvert(Provider, value).Cast<T>()
				;
		}

		public virtual IBasicValueConverter GetConverterFor(object value, Type toType)
		{
			if (value == null)
			{
				if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
					return new DefaultValueConverter(toType);
				if (toType.IsValueType)
					return new FailValueConverter(toType);
				return new DefaultValueConverter(toType);
			}

			Type fromType = value.GetType();
			return Provider.GetBasicConverter(fromType, toType);
		}
	}
}
