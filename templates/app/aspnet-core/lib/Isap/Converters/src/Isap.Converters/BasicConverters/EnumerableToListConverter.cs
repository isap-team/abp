using System;
using System.Collections;
using System.Collections.Generic;
using Isap.Converters.Extensions;

namespace Isap.Converters.BasicConverters
{
	public static class EnumerableToListConverter
	{
		public static IBasicValueConverter Create(Type elementType)
		{
			Type converterType = typeof(EnumerableToListConverter<>).MakeGenericType(elementType);
			return (IBasicValueConverter) Activator.CreateInstance(converterType);
		}
	}

	public class EnumerableToListConverter<TItem>: BasicValueConverterBase<IEnumerable, List<TItem>>
	{
		public override ConvertAttempt<List<TItem>> TryConvert(IBasicValueConverterProvider converterProvider, IEnumerable value)
		{
			List<TItem> result = new List<TItem>();
			foreach (object item in value)
			{
				switch (item)
				{
					case null:
						result.Add((TItem) typeof(TItem).GetDefaultValue());
						break;
					default:
						IBasicValueConverter converter = converterProvider.GetBasicConverter(item.GetType(), typeof(TItem));
						ConvertAttempt<TItem> attempt = converter.TryConvert(converterProvider, item).Cast<TItem>();
						if (!attempt.IsSuccess)
							return ConvertAttempt.Fail<List<TItem>>();
						result.Add(attempt.Result);
						break;
				}
			}

			return ConvertAttempt.Success(result);
		}
	}
}
