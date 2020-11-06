using Isap.CommonCore.Utils;
using Xunit;

namespace Isap.CommonCore.Tests.Utils
{
	public class TimeZoneHelpersTest
	{
		[Theory]
		[InlineData("Europe/Moscow", TimeZoneIdSystemType.IANA, TimeZoneIdSystemType.Windows, "Russian Standard Time")]
		[InlineData("Russian Standard Time", TimeZoneIdSystemType.Windows, TimeZoneIdSystemType.IANA, "Europe/Moscow")]
		public void ConvertTimeZoneIdTest(string timeZoneId, TimeZoneIdSystemType fromType, TimeZoneIdSystemType toType, string expectedTimeZoneId)
		{
			string actual = TimeZoneHelpers.ConvertTimeZoneId(timeZoneId, fromType, toType);
			Assert.Equal(expectedTimeZoneId, actual);
		}
	}
}
