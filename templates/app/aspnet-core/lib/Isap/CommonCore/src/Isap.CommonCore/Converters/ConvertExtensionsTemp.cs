using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Isap.CommonCore.Converters.CollectionConverters;
using Isap.Converters;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Converters
{
	public static class ConvertExtensionsTemp
	{
		private static readonly ConcurrentDictionary<Type, IObjectConverter> _objectConverters =
			new ConcurrentDictionary<Type, IObjectConverter>(new Dictionary<Type, IObjectConverter>
				{
					{ typeof(string), new ObjectToStringConverter() },
					{ typeof(byte[]), new ObjectToByteArrayConverter() },
				});

		private static readonly Dictionary<Type, Func<Type, IObjectConverter>> _genericCollectionConverters =
			new Dictionary<Type, Func<Type, IObjectConverter>>
				{
					{ typeof(List<>), EnumerableToListConverter.GetOrCreateConverter },
					{ typeof(IList<>), EnumerableToListConverter.GetOrCreateConverter },
				};

		public static ConvertAttempt TryConvertTo(object source, Type toType)
		{
			return ConvertInvoker.GetOrCreateInvoker(toType).TryConvertTo(source);
		}

		public static ConvertAttempt<T> TryConvertTo<T>(object source)
		{
			Type toType = typeof(T);
			if (toType.GetTypeInfo().IsValueType)
				return TryConvertToValueType<T>(source);
			return TryConvertToObject<T>(source);
		}

		private static ConvertAttempt<T> TryConvertToObject<T>(object source)
		{
			switch (source)
			{
				case null:
					return ConvertAttempt.Success(default(T));
				case T targetTypeValue:
					return ConvertAttempt.Success(targetTypeValue);
				case JValue _:
					break;
				case string _:
					break;
				case IEnumerable enumerable:
					return TryConvertEnumerableToObject<T>(enumerable);
			}

			if (!_objectConverters.TryGetValue(typeof(T), out IObjectConverter converter))
				return ConvertAttempt.Fail<T>();
			return converter.TryConvert(source).Cast<T>();
		}

		private static ConvertAttempt<T> TryConvertEnumerableToObject<T>(IEnumerable source)
		{
			Type toType = typeof(T);
			if (toType.IsArray)
			{
				Type elementType = toType.GetElementType();
				IObjectConverter converter = EnumerableToArrayConverter.GetOrCreateConverter(elementType);
				return converter.TryConvert(source).Cast<T>();
			}

			if (toType.IsGenericType)
			{
				if (_genericCollectionConverters.TryGetValue(toType.GetGenericTypeDefinition(), out Func<Type, IObjectConverter> create))
				{
					Type elementType = toType.GetGenericArguments()[0];
					IObjectConverter converter = create(elementType);
					return converter.TryConvert(source).Cast<T>();
				}
			}

			throw new NotImplementedException();
		}

		private static ConvertAttempt<T> TryConvertToValueType<T>(object source)
		{
			if (source == null)
				return ConvertAttempt.Fail<T>();
			Type fromType = source.GetType();
			if (typeof(T).IsAssignableFrom(fromType))
				return ConvertAttempt.Success((T) source);
			switch (source)
			{
				case string stringValue:
					return new StringToValueTypeConverter().TryConvertTo<T>(stringValue);
				case byte[] value:
					return new ByteArrayToValueConverter().TryConvertTo<T>(value);
				case ValueType value:
					return new ValueTypeToValueTypeConverter().TryConvertTo<T>(value);
				default:
					throw new InvalidOperationException();
			}
		}

		public static T ConvertTo<T>(object source)
		{
			ConvertAttempt<T> attempt = TryConvertTo<T>(source);
			if (!attempt.IsSuccess)
				throw new InvalidOperationException($"Can't convert value '{source}' to type '{typeof(T)}'.");
			return attempt.Result;
		}

		public static object ConvertTo(object source, Type toType)
		{
			ConvertAttempt attempt = TryConvertTo(source, toType);
			if (!attempt.IsSuccess)
				throw new InvalidOperationException($"Can't convert value '{source}' to type '{toType}'.");
			return attempt.Result;
		}
	}
}
