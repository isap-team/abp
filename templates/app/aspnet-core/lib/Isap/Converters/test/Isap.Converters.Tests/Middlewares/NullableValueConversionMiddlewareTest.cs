using System;
using Isap.Converters.BasicConverters;
using Isap.Converters.Middlewares;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests.Middlewares
{
	public class NullableValueConversionMiddlewareTest
	{
		[Theory]
		[InlineData(typeof(string), typeof(bool?), typeof(NullableValueTypeConverter<bool>))]
		[InlineData(typeof(string), typeof(int?), typeof(NullableValueTypeConverter<int>))]
		public void GetBasicConverterTest(Type fromType, Type toType, Type expectedConverterType)
		{
			var target = new NullableValueConversionMiddleware(new FailoverValueConversionMiddleware());
			IBasicValueConverter basicConverter = target.GetBasicConverter(fromType, toType);
			basicConverter.ShouldNotBeNull();
			basicConverter.ShouldBeOfType(expectedConverterType);
		}
	}
}
