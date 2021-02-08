using System;

namespace Isap.CommonCore.Utils
{
	public struct TimeZoneId
	{
		public static readonly TimeZoneId UtcTimeZoneId = new TimeZoneId(TimeZoneHelpers.GetOSTimeZoneIdSystem(), TimeZoneHelpers.UtcTimeZoneId);

		public TimeZoneId(TimeZoneIdSystemType idSystem, string value)
		{
			IdSystem = idSystem;
			Value = value;
		}

		public TimeZoneIdSystemType IdSystem { get; set; }
		public string Value { get; set; }

		public TimeZoneId ConvertTo(TimeZoneIdSystemType idSystem)
		{
			return new TimeZoneId(idSystem, TimeZoneHelpers.ConvertTimeZoneId(Value, IdSystem, idSystem));
		}

		public TimeZoneInfo GetTimeZoneInfo()
		{
			TimeZoneId timeZoneId = ConvertTo(TimeZoneHelpers.GetOSTimeZoneIdSystem());
			return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Value);
		}

		public override string ToString()
		{
			return $"{Value} (IdSystem: {IdSystem})";
		}
	}
}
