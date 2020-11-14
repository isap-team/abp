using System;
using Isap.CommonCore.Intervals;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Intervals
{
	public class DateIntervalValueTypeDescriptorTest
	{
		[Fact]
		public void CtorTest()
		{
			var target = new DateIntervalValueTypeDescriptor();
			target.Comparer.ShouldNotBeNull();
			target.MinValue.ShouldBe(new DateTime(1, 1, 1));
			target.MaxValue.ShouldBe(new DateTime(9999, 12, 31));
		}

		[Fact]
		public void GetNextValueTest()
		{
			var target = new DateIntervalValueTypeDescriptor();
			DateTime actual = target.GetNextValue(new DateTime(2018, 1, 1));
			actual.ShouldBe(new DateTime(2018, 1, 2));
		}

		[Fact]
		public void GetPrevValueTest()
		{
			var target = new DateIntervalValueTypeDescriptor();
			DateTime actual = target.GetPrevValue(new DateTime(2018, 1, 1));
			actual.ShouldBe(new DateTime(2017, 12, 31));
		}

		[Theory]
		[InlineData("2017-12-31", "2018-01-01", "2018-01-31", false)]
		[InlineData("2018-02-01", "2018-01-01", "2018-01-31", false)]
		[InlineData("2018-01-01", "2018-01-01", "2018-01-31", true)]
		[InlineData("2018-01-02", "2018-01-01", "2018-01-31", true)]
		[InlineData("2018-01-31", "2018-01-01", "2018-01-31", true)]
		public void BetweenTest(string value, string fromValue, string toValue, bool expectedResult)
		{
			InternalBetweenTest(DateTime.Parse(value), DateTime.Parse(fromValue), DateTime.Parse(toValue), expectedResult);
		}

		private void InternalBetweenTest(DateTime value, DateTime fromValue, DateTime toValue, bool expectedResult)
		{
			var target = new DateIntervalValueTypeDescriptor();
			target.Between(value, fromValue, toValue).ShouldBe(expectedResult);
		}
	}
}
