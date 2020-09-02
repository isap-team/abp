using System;
using Isap.Converters.BasicConverters;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests.BasicConverters
{
	public class ValueTypeParserTest
	{
		[Theory]
		[InlineData("00:01:00", "00:01:00")]
		[InlineData("1.00:01:00", "1.00:01:00")]
		public void TryConvertSuccessTest(string fromValue, string expectedResult)
		{
			InternalTryConvertSuccessTest(fromValue, TimeSpan.Parse(expectedResult));
		}

		private void InternalTryConvertSuccessTest(string fromValue, TimeSpan expectedResult)
		{
			IBasicValueConverter<string, TimeSpan> target = new ValueTypeParser<TimeSpan>(TimeSpan.TryParse);
			ConvertAttempt attempt = target.TryConvert(null, fromValue);
			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(true);
			attempt.Result.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("hello")]
		public void TryConvertFailTest(string fromValue)
		{
			IBasicValueConverter<string, TimeSpan> target = new ValueTypeParser<TimeSpan>(TimeSpan.TryParse);
			ConvertAttempt attempt = target.TryConvert(null, fromValue);
			attempt.ShouldNotBeNull();
			attempt.IsSuccess.ShouldBe(false);
		}
	}
}
