using Isap.CommonCore.Utils;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Utils
{
	public class FullNameTest
	{
		[Theory]
		[InlineData("Иванов Иван Иванович", true, "Иванов", "Иван", "Иванович")]
		[InlineData("Иванов Иван", true, "Иванов", "Иван", null)]
		[InlineData("Иванов", true, "Иванов", null, null)]
		[InlineData("Иванов23", false, null, null, null)]
		[InlineData("Иванов Иван23", false, null, null, null)]
		[InlineData("Иванов Иван Иванович23", false, null, null, null)]
		[InlineData("Римский-Корсаков Иван Иванович", true, "Римский-Корсаков", "Иван", "Иванович")]
		[InlineData("Д'Артаньян Иван Иванович", true, "Д'Артаньян", "Иван", "Иванович")]
		[InlineData("Иванов Иван_ Иванович", false, null, null, null)]
		[InlineData("Иванов Иван Иванович%", false, null, null, null)]
		public void TryParseTest(string valueToParse, bool isSuccess, string lastName, string firstName, string middleName)
		{
			bool isSuccessResult = FullName.TryParse(valueToParse, out var result);
			if (isSuccess)
			{
				isSuccessResult.ShouldBe(true);
				result.ShouldNotBeNull();
				result.FirstName.ShouldBe(firstName);
				result.MiddleName.ShouldBe(middleName);
				result.LastName.ShouldBe(lastName);
			}
			else
			{
				isSuccessResult.ShouldBe(false);
				result.ShouldBeNull();
			}
		}

		[Theory]
		[InlineData("Иванов Иван Иванович ", true, "Иванов", "Иван", "Иванович")]
		[InlineData("Иванов Иван ", true, "Иванов", "Иван", null)]
		[InlineData("Иванов ", true, "Иванов", null, null)]
		[InlineData("Иванов23", false, null, null, null)]
		[InlineData("Иванов Иван23", false, null, null, null)]
		[InlineData("Иванов Иван Иванович23", false, null, null, null)]
		[InlineData("Римский-Корсаков Иван Иванович ", true, "Римский-Корсаков", "Иван", "Иванович")]
		[InlineData("Д'Артаньян Иван Иванович ", true, "Д'Артаньян", "Иван", "Иванович")]
		[InlineData("Иванов Иван_ Иванович ", true, "Иванов", "Иван_", "Иванович")]
		[InlineData("Иванов Иван Иванович%", true, "Иванов", "Иван", "Иванович%")]
		[InlineData("Иванов Иван Иванович", true, "Иванов", "Иван", "Иванович%")]
		[InlineData("Иванов Иван", true, "Иванов", "Иван%", null)]
		[InlineData("Иванов", true, "Иванов%", null, null)]
		public void TryParseForSearchTest(string valueToParse, bool isSuccess, string lastName, string firstName, string middleName)
		{
			bool isSuccessResult = FullName.TryParseForSearch(valueToParse, out var result);
			if (isSuccess)
			{
				isSuccessResult.ShouldBe(true);
				result.ShouldNotBeNull();
				result.FirstName.ShouldBe(firstName);
				result.MiddleName.ShouldBe(middleName);
				result.LastName.ShouldBe(lastName);
			}
			else
			{
				isSuccessResult.ShouldBe(false);
				result.ShouldBeNull();
			}
		}

		[Theory]
		[InlineData("Иванов", "Иван", "Иванович", "Иванов И. И.")]
		[InlineData("Иванов", "Иван", null, "Иванов И.")]
		[InlineData("Иванов", null, null, "Иванов")]
		[InlineData(null, null, null, "")]
		[InlineData("Римский-Корсаков", null, "Иванович", "Римский-Корсаков")]
		[InlineData(null, "Иван", "Иванович", " И. И.")]
		public void ShortNameTest(string lastName, string firstName, string middleName, string shortName)
		{
			var fullName = new FullName(lastName, firstName, middleName);
			fullName.ShortName.ShouldBe(shortName);
		}
	}
}
