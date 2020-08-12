using System;
using Isap.Converters.BasicConverters;
using Isap.Converters.Middlewares;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests.Middlewares
{
	public class StringToValueTypeConversionMiddlewareTest
	{
		[Theory]
		[InlineData(typeof(string), typeof(bool), typeof(StringToBooleanConverter))]
		[InlineData(typeof(string), typeof(int), typeof(ValueTypeParser<int>))]
		//[InlineData(typeof(int), (long) int.MaxValue, int.MaxValue)]
		public void GetBasicConverterTest(Type fromType, Type toType, Type expectedConverterType)
		{
			var target = new StringToValueTypeConversionMiddleware(new FailoverValueConversionMiddleware());
			IBasicValueConverter basicConverter = target.GetBasicConverter(fromType, toType);
			basicConverter.ShouldNotBeNull();
			basicConverter.ShouldBeOfType(expectedConverterType);
		}
	}
}
