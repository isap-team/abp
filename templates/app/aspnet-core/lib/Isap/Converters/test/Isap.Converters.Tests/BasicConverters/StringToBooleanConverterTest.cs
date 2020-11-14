using Isap.Converters.BasicConverters;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests.BasicConverters
{
	public class StringToBooleanConverterTest
	{
		[Theory]
		[InlineData("false", false)]
		[InlineData("0", false)]
		[InlineData("true", true)]
		[InlineData("1", true)]
		public void TryConvertSuccessTest(string fromValue, bool expectedResult)
		{
			IBasicValueConverter<string, bool> target = new StringToBooleanConverter();
			ConvertAttempt attempt = target.TryConvert(null, fromValue);
			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(true);
			attempt.Result.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData("hello")]
		[InlineData("2.4")]
		[InlineData(null)]
		public void TryConvertFailTest(string fromValue)
		{
			IBasicValueConverter<string, bool> target = new StringToBooleanConverter();
			ConvertAttempt attempt = target.TryConvert(null, fromValue);
			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(false);
		}
	}
}
