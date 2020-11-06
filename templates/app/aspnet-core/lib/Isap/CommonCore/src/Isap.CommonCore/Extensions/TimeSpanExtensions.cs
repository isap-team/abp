using System;

namespace Isap.CommonCore.Extensions
{
	public static class TimeSpanExtensions
	{
		public static double TotalYears(this TimeSpan value)
		{
			return value.TotalDays / 365.2425D;
		}
	}
}
