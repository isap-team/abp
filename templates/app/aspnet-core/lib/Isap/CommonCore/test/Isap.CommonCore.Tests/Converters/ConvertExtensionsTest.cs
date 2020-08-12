using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Isap.CommonCore.Converters;
using Isap.CommonCore.Utils;
using Isap.Converters;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Isap.CommonCore.Tests.Converters
{
	public class ConvertExtensionsTest
	{
		public enum GenderType
		{
			Male = 0,
			Female = 1,
		}

		[Theory]
		[InlineData(typeof(bool), "false", false)]
		[InlineData(typeof(bool), "0", false)]
		[InlineData(typeof(bool), "true", true)]
		[InlineData(typeof(bool), "1", true)]
		[InlineData(typeof(MethodImplAttributes), "AggressiveInlining", MethodImplAttributes.AggressiveInlining)]
		[InlineData(typeof(MethodImplAttributes), "3", MethodImplAttributes.CodeTypeMask)]
		[InlineData(typeof(MethodImplAttributes), "259", MethodImplAttributes.AggressiveInlining | MethodImplAttributes.CodeTypeMask)]
		[InlineData(typeof(TimeSpan), "00:01:00", "00:01:00")]
		[InlineData(typeof(GenderType), "1", GenderType.Female)]
		[InlineData(typeof(GenderType?), "1", GenderType.Female)]
		[InlineData(typeof(int), (long) int.MaxValue, int.MaxValue)]
		public void TryConvertSuccessTest(Type toType, object fromValue, object expectedResult)
		{
			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(fromValue, toType);
			Assert.NotNull(attempt);
			Assert.True(attempt.IsSuccess);

			Type expectedResultType = expectedResult?.GetType();
			if (attempt.ResultType != expectedResultType)
			{
				attempt = ConvertExtensionsTemp.TryConvertTo(attempt.Result, expectedResultType);
				Assert.NotNull(attempt);
				Assert.True(attempt.IsSuccess);
			}

			Assert.Equal(expectedResult, attempt.Result);
		}

		[Theory]
		[InlineData("dummy", typeof(List<string>))]
		public void TryConvertToListFailTest(object source, Type targetType)
		{
			Assert.True(targetType.IsGenericType);
			Assert.Contains(targetType.GetGenericTypeDefinition(), new[] { typeof(List<>), typeof(IList<>) });

			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(source, targetType);
			Assert.NotNull(attempt);
			Assert.False(attempt.IsSuccess);
		}

		[Theory]
		[InlineData(new[] { "dummy" }, typeof(List<string>), "dummy")]
		public void TryConvertToListSuccessTest(object source, Type targetType, params object[] expectedResultItems)
		{
			Assert.True(targetType.IsGenericType);
			Assert.Contains(targetType.GetGenericTypeDefinition(), new[] { typeof(List<>), typeof(IList<>) });

			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(source, targetType);
			Assert.NotNull(attempt);
			Assert.True(attempt.IsSuccess);

			Type elementType = targetType.GetGenericArguments()[0];

			Assert.IsType(targetType, attempt.Result);
			List<object> result = ((IEnumerable) attempt.Result).Cast<object>().ToList();
			Assert.Equal(expectedResultItems.Length, result.Count);
			List<(object Expected, object Actual)> tuples = expectedResultItems.Zip(result, (o, o1) => (Expected: o, Actual: o1)).ToList();
			foreach ((object Expected, object Actual) tuple in tuples)
				Assert.Equal(ConvertExtensionsTemp.ConvertTo(tuple.Expected, elementType), tuple.Actual);
		}

		[Theory]
		[InlineData(new[] { "123" }, typeof(int[]), 123)]
		public void TryConvertToArraySuccessTest(object source, Type targetType, params object[] expectedResultItems)
		{
			Assert.True(targetType.IsArray);

			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(source, targetType);
			Assert.NotNull(attempt);
			Assert.True(attempt.IsSuccess);

			Type elementType = targetType.GetElementType();
			Assert.IsType(targetType, attempt.Result);
			List<object> result = ((IEnumerable) attempt.Result).Cast<object>().ToList();
			Assert.Equal(expectedResultItems.Length, result.Count);
			List<(object Expected, object Actual)> tuples = expectedResultItems.Zip(result, (o, o1) => (Expected: o, Actual: o1)).ToList();
			foreach ((object Expected, object Actual) tuple in tuples)
				Assert.Equal(ConvertExtensionsTemp.ConvertTo(tuple.Expected, elementType), tuple.Actual);
		}

		[Theory]
		[InlineData("[\"Прекращен\", \"Статус приостановлен\", \"Исключен\"]", typeof(List<string>), "Прекращен", "Статус приостановлен", "Исключен")]
		public void TryConvertJsonToListSuccessTest(string json, Type targetType, params object[] expectedResultItems)
		{
			// Arrange
			object source = JToken.Parse(json);

			// Action
			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(source, targetType);

			// Verify
			Assert.NotNull(attempt);
			Assert.True(attempt.IsSuccess);

			Type elementType = targetType.GetGenericArguments()[0];

			Assert.IsType(targetType, attempt.Result);
			List<object> result = ((IEnumerable) attempt.Result).Cast<object>().ToList();
			Assert.Equal(expectedResultItems.Length, result.Count);
			List<(object Expected, object Actual)> tuples = expectedResultItems.Zip(result, (o, o1) => (Expected: o, Actual: o1)).ToList();
			foreach ((object Expected, object Actual) tuple in tuples)
				Assert.Equal(ConvertExtensionsTemp.ConvertTo(tuple.Expected, elementType), tuple.Actual);
		}

		[Theory]
		[InlineData(typeof(Guid), "66A6905F-4C56-45DD-A967-2A115B361644", "5F90A666564CDD45A9672A115B361644")]
		public void ConvertToByteArrayTest(Type sourceType, object sourceValue, string expectedResult)
		{
			// Arrange
			sourceValue = ConvertExtensionsTemp.ConvertTo(sourceValue, sourceType);

			// Action
			ConvertAttempt<byte[]> attempt = ConvertExtensionsTemp.TryConvertTo<byte[]>(sourceValue);

			// Verify
			attempt.IsSuccess.ShouldBe(true);
			HexUtility.ByteArrayToString(attempt.Result).ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(typeof(Guid), "66A6905F-4C56-45DD-A967-2A115B361644", "5F90A666564CDD45A9672A115B361644")]
		public void ConvertFromByteArrayTest(Type targetType, object expectedValue, string sourceHexValue)
		{
			// Arrange
			expectedValue = ConvertExtensionsTemp.ConvertTo(expectedValue, targetType);
			byte[] sourceValue = HexUtility.StringToByteArray(sourceHexValue);

			// Action
			ConvertAttempt attempt = ConvertExtensionsTemp.TryConvertTo(sourceValue, targetType);

			// Verify
			attempt.IsSuccess.ShouldBe(true);
			attempt.Result.ShouldBe(expectedValue);
		}
	}
}
