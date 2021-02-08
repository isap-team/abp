using System;
using Isap.CommonCore.Utils;

namespace Isap.CommonCore.Extensions
{
	public static class DateTimeExtensions
	{
		public static readonly DateTime MinSupportedDate = new DateTime(1, 1, 1);
		public static readonly DateTime MaxSupportedDate = new DateTime(9999, 12, 31);
		public static readonly DateTime UnixTimestampZeroDate = new DateTime(1970, 1, 1);

		public static int ToUnixTimestamp(this DateTime value, DateTimeKind? checkKindWhenHasValue = null)
		{
			DateTimeKind dateTimeKind = checkKindWhenHasValue ?? value.Kind;
			if (value.Kind != dateTimeKind)
				throw new InvalidOperationException($"Specified value '{value}' should be with kind = '{dateTimeKind}' but actual kind is '{value.Kind}'.");
			return (int) (value - DateTime.SpecifyKind(UnixTimestampZeroDate, dateTimeKind)).TotalSeconds;
		}

		public static DateTime FromUnixTimestamp(this int value, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			return DateTime.SpecifyKind(UnixTimestampZeroDate.AddSeconds(value), kind);
		}

		public static DateTime ToUtc(this DateTime value)
		{
			switch (value.Kind)
			{
				case DateTimeKind.Unspecified:
					return DateTime.SpecifyKind(value, DateTimeKind.Utc);
				case DateTimeKind.Local:
					return value.ToUniversalTime();
				case DateTimeKind.Utc:
					return value;
				default:
					throw new NotSupportedException();
			}
		}

		public static DateTime? ConvertTime(this DateTime? dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			if (!dateTime.HasValue)
				return null;

			return dateTime.Value.ConvertTime(sourceTimeZoneId, destinationTimeZoneId);
		}

		public static DateTime? ConvertTime(this DateTime? dateTime, TimeZoneId sourceTimeZoneId, TimeZoneId destinationTimeZoneId)
		{
			if (!dateTime.HasValue)
				return null;

			return dateTime.Value.ConvertTime(sourceTimeZoneId, destinationTimeZoneId);
		}

		public static DateTime ConvertTime(this DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId);
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), sourceTimeZone, destinationTimeZone);
		}

		public static DateTime ConvertTime(this DateTime dateTime, TimeZoneId sourceTimeZoneId, TimeZoneId destinationTimeZoneId)
		{
			TimeZoneInfo sourceTimeZone = sourceTimeZoneId.GetTimeZoneInfo();
			TimeZoneInfo destinationTimeZone = destinationTimeZoneId.GetTimeZoneInfo();
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), sourceTimeZone, destinationTimeZone);
		}

		public static DateTime? ConvertTime(this DateTime? dateTime, string toTimeZoneId)
		{
			if (!dateTime.HasValue)
				return null;

			return ConvertTime(dateTime.Value, toTimeZoneId);
		}

		public static DateTime? ConvertTime(this DateTime? dateTime, TimeZoneId toTimeZoneId)
		{
			if (!dateTime.HasValue)
				return null;

			return ConvertTime(dateTime.Value, toTimeZoneId);
		}

		public static DateTime ConvertTime(this DateTime dateTime, string toTimeZoneId)
		{
			DateTime utcTime = dateTime.ToUtc();
			TimeZoneInfo toTimeZone = TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneId);
			return TimeZoneInfo.ConvertTime(utcTime, toTimeZone);
		}

		public static DateTime ConvertTime(this DateTime dateTime, TimeZoneId toTimeZoneId)
		{
			DateTime utcTime = dateTime.ToUtc();
			TimeZoneInfo toTimeZone = toTimeZoneId.GetTimeZoneInfo();
			return TimeZoneInfo.ConvertTime(utcTime, toTimeZone);
		}

		public static DateTime ConvertFromServerToClientTime(this DateTime dateTime, string clientTimeZoneId)
		{
			if (dateTime.Kind != DateTimeKind.Utc)
				throw new ArgumentException($"DateTime have unexpected kind '{dateTime.Kind}' when expected kind is '{DateTimeKind.Utc}'.");
			return dateTime.ConvertTime(clientTimeZoneId);
		}

		public static DateTime ConvertFromServerToClientTime(this DateTime dateTime, TimeZoneId clientTimeZoneId)
		{
			if (dateTime.Kind != DateTimeKind.Utc)
				throw new ArgumentException($"DateTime have unexpected kind '{dateTime.Kind}' when expected kind is '{DateTimeKind.Utc}'.");
			return dateTime.ConvertTime(clientTimeZoneId);
		}

		public static DateTime ConvertFromClientToServerTime(this DateTime dateTime, string clientTimeZoneId)
		{
			switch (dateTime.Kind)
			{
				case DateTimeKind.Unspecified:
					// похоже что при использовании IANA нужно дополнительно вызывать метод DateTime.SpecifyKind
					return DateTime.SpecifyKind(dateTime.ConvertTime(clientTimeZoneId, TimeZoneHelpers.UtcTimeZoneId), DateTimeKind.Utc);
				case DateTimeKind.Utc:
					return dateTime;
				default:
					throw new ArgumentException(
						$"DateTime have unexpected kind '{dateTime.Kind}' when expected kind is '{DateTimeKind.Unspecified}' or '{DateTimeKind.Utc}'.");
			}
		}

		public static DateTime ConvertFromClientToServerTime(this DateTime dateTime, TimeZoneId clientTimeZoneId)
		{
			switch (dateTime.Kind)
			{
				case DateTimeKind.Unspecified:
					// похоже что при использовании IANA нужно дополнительно вызывать метод DateTime.SpecifyKind
					return DateTime.SpecifyKind(dateTime.ConvertTime(clientTimeZoneId, TimeZoneId.UtcTimeZoneId), DateTimeKind.Utc);
				case DateTimeKind.Utc:
					return dateTime;
				default:
					throw new ArgumentException(
						$"DateTime have unexpected kind '{dateTime.Kind}' when expected kind is '{DateTimeKind.Unspecified}' or '{DateTimeKind.Utc}'.");
			}
		}

		public static DateTime MonthsFirstDate(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static DateTime MonthsLastDate(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
		}
	}
}
