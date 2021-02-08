using System;
using System.Collections;
using System.Collections.Generic;

namespace Isap.Converters.BasicConverters
{
	public static class EnumerableToArrayConverter
	{
		public static IBasicValueConverter Create(Type elementType)
		{
			Type converterType = typeof(EnumerableToArrayConverter<>).MakeGenericType(elementType);
			return (IBasicValueConverter) Activator.CreateInstance(converterType);
		}
	}

	public class EnumerableToArrayConverter<TItem>: BasicValueConverterBase<IEnumerable, TItem[]>
	{
		public override ConvertAttempt<TItem[]> TryConvert(IBasicValueConverterProvider converterProvider, IEnumerable value)
		{
			var converter = new EnumerableToListConverter<TItem>();
			ConvertAttempt<List<TItem>> attempt = converter.TryConvert(converterProvider, value);
			return attempt.IsSuccess
					? ConvertAttempt.Success(attempt.Result.ToArray())
					: ConvertAttempt.Fail<TItem[]>()
				;
		}
	}
}
