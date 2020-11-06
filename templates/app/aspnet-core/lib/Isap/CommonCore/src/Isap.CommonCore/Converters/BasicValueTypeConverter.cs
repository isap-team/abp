using System;
using Isap.Converters;

namespace Isap.CommonCore.Converters
{
	public class BasicValueTypeConverter<TSource, TTarget>: ValueTypeConverterBase<TSource, TTarget>
		where TTarget: struct
	{
		private readonly Func<TSource, TTarget> _convert;

		public BasicValueTypeConverter(Func<TSource, TTarget> convert)
		{
			_convert = convert;
		}

		public override ConvertAttempt<TTarget> TryConvert(TSource sourceValue)
		{
			try
			{
				return ConvertAttempt.Success(_convert(sourceValue));
			}
			catch (Exception)
			{
				return ConvertAttempt.Fail<TTarget>();
			}
		}
	}
}
