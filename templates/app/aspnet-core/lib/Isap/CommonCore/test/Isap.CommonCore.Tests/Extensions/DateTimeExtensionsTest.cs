using System;
using System.Globalization;
using Isap.CommonCore.Extensions;
using Isap.CommonCore.Utils;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
// ReSharper disable PossibleInvalidOperationException

namespace Isap.CommonCore.Tests.Extensions
{
	public class DateTimeExtensionsTest
	{
		public const string UtcTimeZoneId = "UTC";
		public const string RussianStandardTimeZoneId = "Russian Standard Time";
		private readonly ITestOutputHelper _output;

		public DateTimeExtensionsTest(ITestOutputHelper output)
		{
			_output = output;
		}

		[Theory]
		[InlineData("2017-12-31", DateTimeKind.Utc, 1514678400)]
		[InlineData("2017-12-31", DateTimeKind.Local, 1514678400)]
		[InlineData("2017-12-31", DateTimeKind.Unspecified, 1514678400)]
		public void ToUnixTimestampTest(string sourceTime, DateTimeKind kind, int expectedTimestamp)
		{
			DateTime sourceDateTime = ParseDateTime(sourceTime, kind) ?? DateTime.MinValue;
			InternalToUnixTimestampTest(sourceDateTime, expectedTimestamp);
		}

		private void InternalToUnixTimestampTest(DateTime sourceTime, int expectedTimestamp)
		{
			int actualTimestamp = sourceTime.ToUnixTimestamp();
			Assert.Equal(expectedTimestamp, actualTimestamp);
		}

		[Theory]
		[InlineData(1514678400, "2017-12-31T00:00:00", DateTimeKind.Unspecified)]
		[InlineData(1514678400, "2017-12-31T00:00:00", DateTimeKind.Utc)]
		[InlineData(1514678400, "2017-12-31T00:00:00", DateTimeKind.Local)]
		public void FromUnixTimestampTest(int sourceTimestamp, string expectedTime, DateTimeKind expectedKind)
		{
			DateTime actualTime = sourceTimestamp.FromUnixTimestamp(expectedKind);
			Assert.Equal(expectedTime, actualTime.ToString("s"));
			Assert.Equal(expectedKind, actualTime.Kind);
		}

		[Theory]
		[InlineData(null, DateTimeKind.Unspecified, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, null)]
		[InlineData("2017-12-31", DateTimeKind.Unspecified, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31", DateTimeKind.Utc, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31", DateTimeKind.Local, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T00:00:00")]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Unspecified, TimeZoneIdSystemType.Windows, UtcTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Utc, TimeZoneIdSystemType.Windows, UtcTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Local, TimeZoneIdSystemType.Windows, UtcTimeZoneId, "2017-12-31T00:00:00")]
		public void ConvertTimeTest(string sourceTime, DateTimeKind kind, TimeZoneIdSystemType timeZoneIdSystem,
			string toTimeZoneId, string expectedResult)
		{
			TimeZoneIdSystemType currentTimeZoneIdSystemType = TimeZoneHelpers.GetOSTimeZoneIdSystem();
			toTimeZoneId = TimeZoneHelpers.ConvertTimeZoneId(toTimeZoneId, timeZoneIdSystem, currentTimeZoneIdSystemType);
			DateTime? sourceDateTime = ParseDateTime(sourceTime, kind);
			InternalConvertTimeTest(sourceDateTime, toTimeZoneId, expectedResult);
		}

		private static void InternalConvertTimeTest(DateTime? sourceTime, string destinationTimeZoneId, string expectedResult)
		{
			DateTime? actualResult = sourceTime.ConvertTime(destinationTimeZoneId);
			var actualResultAsString = actualResult?.ToString("s");
			Assert.Equal(expectedResult, actualResultAsString);
		}

		private static DateTime? ParseDateTime(string value, DateTimeKind kind)
		{
			if (value == null)
				return null;
			DateTime parsedDateTime;
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
				return DateTime.SpecifyKind(parsedDateTime, kind);
			throw new InvalidOperationException();
		}

		[Theory]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Unspecified, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T00:00:00")]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Utc, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31T03:00:00", DateTimeKind.Local, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, typeof(ArgumentException))]
		public void ConvertFromClientToServerTimeTest(string sourceTime, DateTimeKind kind, TimeZoneIdSystemType timeZoneIdSystem, string clientTimeZoneId,
			object expectedResult)
		{
			TimeZoneIdSystemType currentTimeZoneIdSystemType = TimeZoneHelpers.GetOSTimeZoneIdSystem();
			clientTimeZoneId = TimeZoneHelpers.ConvertTimeZoneId(clientTimeZoneId, timeZoneIdSystem, currentTimeZoneIdSystemType);
			DateTime? sourceDateTime = ParseDateTime(sourceTime, kind);
			InternalConvertFromClientToServerTimeTest(sourceDateTime, clientTimeZoneId, expectedResult);
		}

		private void InternalConvertFromClientToServerTimeTest(DateTime? sourceTime, string clientTimeZoneId, object expectedResult)
		{
			switch (expectedResult)
			{
				case Type expectedExceptionType:
					Exception exception = Assert.Throws(expectedExceptionType, () => sourceTime?.ConvertFromClientToServerTime(clientTimeZoneId));
					_output.WriteLine(exception.ToString());
					break;
				case string expectedStringResult:
					DateTime? actualResult = sourceTime?.ConvertFromClientToServerTime(clientTimeZoneId);
					Assert.True(actualResult.HasValue);
					string actualResultAsString = actualResult.Value.ToString("s");
					Assert.Equal(expectedStringResult, actualResultAsString);
					Assert.Equal(DateTimeKind.Utc, actualResult.Value.Kind);
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		[Theory]
		[InlineData("2017-12-31T00:00:00", DateTimeKind.Unspecified, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, typeof(ArgumentException))]
		[InlineData("2017-12-31T00:00:00", DateTimeKind.Utc, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, "2017-12-31T03:00:00")]
		[InlineData("2017-12-31T00:00:00", DateTimeKind.Local, TimeZoneIdSystemType.Windows, RussianStandardTimeZoneId, typeof(ArgumentException))]
		public void ConvertFromServerToClientTimeTest(string sourceTime, DateTimeKind kind, TimeZoneIdSystemType timeZoneIdSystem, string clientTimeZoneId,
			object expectedResult)
		{
			TimeZoneIdSystemType currentTimeZoneIdSystemType = TimeZoneHelpers.GetOSTimeZoneIdSystem();
			clientTimeZoneId = TimeZoneHelpers.ConvertTimeZoneId(clientTimeZoneId, timeZoneIdSystem, currentTimeZoneIdSystemType);
			DateTime? sourceDateTime = ParseDateTime(sourceTime, kind);
			InternalConvertFromServerToClientTimeTest(sourceDateTime, clientTimeZoneId, expectedResult);
		}

		private void InternalConvertFromServerToClientTimeTest(DateTime? sourceTime, string clientTimeZoneId, object expectedResult)
		{
			switch (expectedResult)
			{
				case Type expectedExceptionType:
					Exception exception = Assert.Throws(expectedExceptionType, () => sourceTime?.ConvertFromServerToClientTime(clientTimeZoneId));
					_output.WriteLine(exception.ToString());
					break;
				case string expectedStringResult:
					DateTime? actualResult = sourceTime?.ConvertFromServerToClientTime(clientTimeZoneId);
					Assert.True(actualResult.HasValue);
					string actualResultAsString = actualResult.Value.ToString("s");
					Assert.Equal(expectedStringResult, actualResultAsString);
					Assert.Equal(DateTimeKind.Unspecified, actualResult.Value.Kind);
					break;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
