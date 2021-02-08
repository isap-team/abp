using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.Converters.CollectionConverters
{
	public static class EnumerableToArrayConverter
	{
		private static readonly ConcurrentDictionary<Type, IObjectConverter> _converters =
			new ConcurrentDictionary<Type, IObjectConverter>();

		public static IObjectConverter Create(Type elementType)
		{
			Type converterType = typeof(EnumerableToArrayConverter<>).MakeGenericType(elementType);
			return (IObjectConverter) Activator.CreateInstance(converterType);
		}

		public static IObjectConverter GetOrCreateConverter(Type toType)
		{
			return _converters.GetOrAdd(toType, Create);
		}
	}

	public class EnumerableToArrayConverter<TElement>: IObjectConverter<IEnumerable, TElement[]>
	{
		public ConvertAttempt<TElement[]> TryConvert(IEnumerable sourceValue)
		{
			IObjectConverter converter = EnumerableToListConverter.GetOrCreateConverter(typeof(TElement));
			ConvertAttempt<List<TElement>> attempt = converter.TryConvert(sourceValue).Cast<List<TElement>>();
			if (!attempt.IsSuccess)
				return ConvertAttempt.Fail<TElement[]>();
			return ConvertAttempt.Success(attempt.Result.ToArray());
		}

		public ConvertAttempt TryConvert(object sourceValue)
		{
			if (sourceValue is IEnumerable enumerable)
				return TryConvert(enumerable);
			return ConvertAttempt.Fail<TElement[]>();
		}
	}
}
