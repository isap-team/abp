using System;
using System.Collections.Generic;
using System.Linq;
using Isap.CommonCore.API;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.API
{
	public class ApiEnumValueHelpersTest
	{
		[Flags]
		public enum TestEnum
		{
			[ApiEnumValue("on")]
			Enabled = 1,

			[ApiEnumValue("off")]
			Disabled = 2,

			[ApiEnumIgnore]
			All = Enabled | Disabled,
		}

		[Theory]
		[InlineData("on,off", "Enabled,Disabled")]
		[InlineData("off,on", "Disabled,Enabled")]
		[InlineData("off,on,on,off", "Disabled,Enabled,Enabled,Disabled")]
		public void ToEnumItemsTest(string input, string expectedItems)
		{
			ToEnumItems(input.Split(',').ToList(), expectedItems.Split(',').ToList());
		}

		private void ToEnumItems(List<string> input, List<string> expectedItems)
		{
			IEnumerable<TestEnum> actual = input.ToEnumItems<TestEnum>();

			actual.Select(i => i.ToString()).ShouldBe(expectedItems);
		}

		[Theory]
		[InlineData("on", TestEnum.Enabled)]
		[InlineData("off", TestEnum.Disabled)]
		[InlineData("on,off", TestEnum.Enabled | TestEnum.Disabled)]
		[InlineData("off,on", TestEnum.Enabled | TestEnum.Disabled)]
		[InlineData("off,on,on,off", TestEnum.Enabled | TestEnum.Disabled)]
		public void ToEnumFlagsTest(string input, TestEnum expectedItems)
		{
			ToEnumFlags(input.Split(',').ToList(), expectedItems);
		}

		private void ToEnumFlags(List<string> input, TestEnum expectedItems)
		{
			TestEnum actual = input.ToEnumFlags<TestEnum>();

			actual.ShouldBe(expectedItems);
		}

		[Theory]
		[InlineData(TestEnum.Enabled, "on")]
		[InlineData(TestEnum.Disabled, "off")]
		[InlineData(TestEnum.Enabled | TestEnum.Disabled, "on,off")]
		public void ToApiEnumValueListTest(TestEnum flags, string expectedItems)
		{
			ToApiEnumValueList(flags, expectedItems.Split(',').ToList());
		}

		private void ToApiEnumValueList(TestEnum flags, List<string> expectedItems)
		{
			IEnumerable<string> actual = flags.ToApiEnumValueList();
			actual.ShouldBe(expectedItems);
		}
	}
}
