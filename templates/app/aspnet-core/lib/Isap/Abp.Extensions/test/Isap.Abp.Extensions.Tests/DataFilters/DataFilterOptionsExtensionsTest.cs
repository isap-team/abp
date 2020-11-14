using System.Collections.Generic;
using System.Globalization;
using Isap.Abp.Extensions.DataFilters;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Isap.Abp.Extensions.Tests.DataFilters
{
	public class DataFilterOptionsExtensionsTest
	{
		[Fact]
		public void DeserializeTest()
		{
			string serialized = JsonConvert.SerializeObject(new Dictionary<string, object>
				{
					{ "DefaultFromValue", decimal.MinValue },
					{ "DefaultToValue", decimal.MaxValue },
				}, new DataFilterValueJsonConverter());
			serialized.ShouldBe(@"{""DefaultFromValue"":""-79228162514264337593543950335"",""DefaultToValue"":""79228162514264337593543950335""}");

			Dictionary<string, object> actual = DataFilterOptionsExtensions.Deserialize(serialized);
			actual.ShouldNotBeNull();
			actual.ContainsKey("DefaultFromValue").ShouldBeTrue();
			actual["DefaultFromValue"].ShouldBeOfType<string>().ShouldBe(decimal.MinValue.ToString(CultureInfo.InvariantCulture));
			actual.ContainsKey("DefaultToValue").ShouldBeTrue();
			actual["DefaultToValue"].ShouldBeOfType<string>().ShouldBe(decimal.MaxValue.ToString(CultureInfo.InvariantCulture));
		}
	}
}
