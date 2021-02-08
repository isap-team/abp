using Isap.CommonCore.Utils;

namespace Isap.CommonCore
{
	public class IsapConsts
	{
		public static readonly TimeZoneId RussianStandardTimeZoneId =
			new TimeZoneId(TimeZoneIdSystemType.Windows, "Russian Standard Time").ConvertTo(TimeZoneHelpers.GetOSTimeZoneIdSystem());
	}
}
