using System;
using Isap.Converters.BasicConverters;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests.Middlewares
{
	public class FailoverValueConversionMiddlewareTest
	{
		[Theory]
		[InlineData(typeof(string), typeof(bool), typeof(FailValueConverter))]
		[InlineData(typeof(string), typeof(string), typeof(SameTypeValueConverter<string>))]
		public void GetBasicConverterTest(Type fromType, Type toType, Type expectedConverterType)
		{
			var target = new FailoverValueConversionMiddleware();
			IBasicValueConverter basicConverter = target.GetBasicConverter(fromType, toType);
			basicConverter.ShouldNotBeNull();
			basicConverter.ShouldBeOfType(expectedConverterType);
		}
	}
}
