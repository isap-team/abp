using System;
using Isap.CommonCore.Extensions;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Extensions
{
	public class GuidExtensionsTest
	{
		[Theory]
		[InlineData(0, "00000000-0000-0000-0000-000000000000")]
		[InlineData(1, "00000001-0000-0000-0000-000000000000")]
		[InlineData(int.MinValue, "80000000-0000-0000-0000-000000000000")]
		[InlineData(int.MaxValue, "7FFFFFFF-0000-0000-0000-000000000000")]
		[InlineData(-1, "FFFFFFFF-0000-0000-0000-000000000000")]
		public void ToGuidTest(int value, string expectedResult)
		{
			Guid actual = value.ToGuid();
			actual.ShouldBe(new Guid(expectedResult));
		}

		[Theory]
		[InlineData("00000000-0000-0000-0000-000000000000", 0)]
		[InlineData("00000001-0000-0000-0000-000000000000", 1)]
		[InlineData("80000000-0000-0000-0000-000000000000", int.MinValue)]
		[InlineData("7FFFFFFF-0000-0000-0000-000000000000", int.MaxValue)]
		[InlineData("FFFFFFFF-0000-0000-0000-000000000000", -1)]
		public void ToIntTest(string value, int expectedResult)
		{
			int actual = new Guid(value).ToInt();
			actual.ShouldBe(expectedResult);
		}
	}
}
