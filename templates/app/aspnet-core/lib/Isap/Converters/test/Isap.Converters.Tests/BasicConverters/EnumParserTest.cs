using System;
using System.Reflection;
using Isap.Converters.BasicConverters;
using Moq;
using Shouldly;
using Xunit;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Isap.Converters.Tests.BasicConverters
{
	public class EnumParserTest
	{
		[Theory]
		[InlineData(typeof(MethodImplAttributes), "AggressiveInlining", MethodImplAttributes.AggressiveInlining)]
		[InlineData(typeof(MethodImplAttributes), "3", MethodImplAttributes.CodeTypeMask)]
		[InlineData(typeof(MethodImplAttributes), "259", MethodImplAttributes.AggressiveInlining | MethodImplAttributes.CodeTypeMask)]
		public void TryConvertSuccessTest(Type toType, string fromValue, object expectedResult)
		{
			var mockRepository = new MockRepository(MockBehavior.Strict);

			var converterProviderMock = mockRepository.Create<IBasicValueConverterProvider>();

			IBasicValueConverter target = EnumParser.Create(toType);

			ConvertAttempt attempt = target.TryConvert(converterProviderMock.Object, fromValue);

			mockRepository.VerifyAll();

			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(true);
			attempt.Result.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(typeof(MethodImplAttributes), null)]
		[InlineData(typeof(MethodImplAttributes), "")]
		[InlineData(typeof(MethodImplAttributes), "XXX")]
		public void TryConvertFailTest(Type toType, string fromValue)
		{
			var mockRepository = new MockRepository(MockBehavior.Strict);

			var converterProviderMock = mockRepository.Create<IBasicValueConverterProvider>();

			IBasicValueConverter target = EnumParser.Create(toType);

			ConvertAttempt attempt = target.TryConvert(converterProviderMock.Object, fromValue);

			mockRepository.VerifyAll();

			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(false);
		}
	}
}
