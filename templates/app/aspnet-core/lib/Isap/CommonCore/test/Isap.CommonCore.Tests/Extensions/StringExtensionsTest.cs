using Isap.CommonCore.Extensions;
using Xunit;

namespace Isap.CommonCore.Tests.Extensions
{
	public class StringExtensionsTest
	{
		[Theory]
		[InlineData(null, 5, "abc", null)]
		[InlineData("12345", 5, "abc", "12345")]
		[InlineData("12345", 4, "abc", "abc5")]
		[InlineData("12345", 3, "abc", "abc")]
		[InlineData("12345", 2, "abc", "bc")]
		[InlineData("12345", 1, "abc", "c")]
		[InlineData("12345", 0, "abc", "")]
		public void TruncStartTest(string value, int maxLength, string truncatedValueMarker, string expectedResults)
		{
			string actual = value.TruncStart(maxLength, truncatedValueMarker);
			Assert.Equal(expectedResults, actual);
		}

		[Theory]
		[InlineData(null, 5, "abc", null)]
		[InlineData("12345", 5, "abc", "12345")]
		[InlineData("12345", 4, "abc", "1abc")]
		[InlineData("12345", 3, "abc", "abc")]
		[InlineData("12345", 2, "abc", "ab")]
		[InlineData("12345", 1, "abc", "a")]
		[InlineData("12345", 0, "abc", "")]
		public void TruncEndTest(string value, int maxLength, string truncatedValueMarker, string expectedResults)
		{
			string actual = value.TruncEnd(maxLength, truncatedValueMarker);
			Assert.Equal(expectedResults, actual);
		}
	}
}
