using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Converters.CollectionConverters
{
	public static class EnumerableToListConverter
	{
		private static readonly ConcurrentDictionary<Type, IObjectConverter> _converters =
			new ConcurrentDictionary<Type, IObjectConverter>();

		public static IObjectConverter Create(Type elementType)
		{
			Type converterType = typeof(EnumerableToListConverter<>).MakeGenericType(elementType);
			return (IObjectConverter) Activator.CreateInstance(converterType);
		}

		public static IObjectConverter GetOrCreateConverter(Type toType)
		{
			return _converters.GetOrAdd(toType, Create);
		}
	}

	public class EnumerableToListConverter<TElement>: IObjectConverter<IEnumerable, List<TElement>>
	{
		public ConvertAttempt<List<TElement>> TryConvert(IEnumerable sourceValue)
		{
			var result = new List<TElement>();
			foreach (object item in sourceValue)
			{
				ConvertAttempt<TElement> attempt = ConvertExtensionsTemp.TryConvertTo<TElement>(item);
				if (!attempt.IsSuccess)
					return ConvertAttempt.Fail<List<TElement>>();
				result.Add(attempt.Result);
			}

			return ConvertAttempt.Success(result);
		}

		public ConvertAttempt TryConvert(object sourceValue)
		{
			if (sourceValue is IEnumerable enumerable)
				return TryConvert(enumerable);
			return ConvertAttempt.Fail<List<TElement>>();
		}
	}
}
