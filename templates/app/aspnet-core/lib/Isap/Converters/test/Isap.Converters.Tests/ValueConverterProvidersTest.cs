using System;
using System.Threading.Tasks;
using Isap.Converters.Middlewares;
using Shouldly;
using Xunit;

namespace Isap.Converters.Tests
{
	public class ValueConverterProvidersTest
	{
		[Fact]
		public void DefaultTopMiddlewareTest()
		{
			IValueConverterProvider target = ValueConverterProviders.Default;

			IValueConversionMiddleware actual = target.TopMiddleware;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<CommonValueConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<EnumerableValueConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<ToStringConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<StringToValueTypeConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<ToEnumConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<JTokenToValueConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<NullableValueConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldNotBeNull();
			actual.ShouldBeOfType<FailoverValueConversionMiddleware>();

			actual = actual.Next;
			actual.ShouldBeNull();
		}

		[Theory]
		[InlineData(null, typeof(string), typeof(bool))]
		public void DefaultConverterFailTest(string input, Type inputType, Type targetType)
		{
			IValueConverter target = ValueConverterProviders.Default.GetConverter();
			object inputValue = target.ConvertTo(inputType, input);
			ConvertAttempt actualAttempt = target.TryConvertTo(targetType, inputValue);
			actualAttempt.ShouldNotBeNull();
			actualAttempt.IsSuccess.ShouldBeFalse();
		}

		[Theory]
		[InlineData(null, typeof(string), typeof(Guid?), null)]
		public void DefaultConverterSuccessTest(string input, Type inputType, Type targetType, string expectedResult)
		{
			IValueConverter target = ValueConverterProviders.Default.GetConverter();
			object inputValue = target.ConvertTo(inputType, input);
			ConvertAttempt actualAttempt = target.TryConvertTo(targetType, inputValue);
			actualAttempt.ShouldNotBeNull();
			actualAttempt.IsSuccess.ShouldBeTrue();
			string actualResult = target.ConvertTo<string>(actualAttempt.Result);
			actualResult.ShouldBe(expectedResult);
		}

		[Fact]
		public void CurrentProviderSyncTest()
		{
			IValueConverter converter = ValueConverterProviders.Default.GetConverter();

			IValueConverterProvider actual = ValueConverterProviders.Current;
			actual.ShouldNotBeNull();
			actual.ShouldBe(ValueConverterProviders.Default);
			actual.ShouldBeOfType<ValueConverterProvider>();
			actual.GetConverter().ShouldNotBe(converter);

			using (ValueConverterProviders.Use(converter))
			{
				actual = ValueConverterProviders.Current;
				actual.ShouldNotBeNull();
				actual.ShouldBeOfType<CachedValueConverterProvider>();
				actual.GetConverter().ShouldBe(converter);
			}

			actual = ValueConverterProviders.Current;
			actual.ShouldNotBeNull();
			actual.ShouldBe(ValueConverterProviders.Default);
			actual.ShouldBeOfType<ValueConverterProvider>();
			actual.GetConverter().ShouldNotBe(converter);
		}

		[Fact]
		public async Task CurrentProviderAsyncTest()
		{
			IValueConverter converter = ValueConverterProviders.Default.GetConverter();

			await Task.Yield();

			IValueConverterProvider actual = ValueConverterProviders.Current;
			actual.ShouldNotBeNull();
			actual.ShouldBe(ValueConverterProviders.Default);
			actual.ShouldBeOfType<ValueConverterProvider>();
			actual.GetConverter().ShouldNotBe(converter);

			await Task.Yield();

			using (ValueConverterProviders.Use(converter))
			{
				await Task.Yield();

				actual = ValueConverterProviders.Current;
				actual.ShouldNotBeNull();
				actual.ShouldBeOfType<CachedValueConverterProvider>();
				actual.GetConverter().ShouldBe(converter);
			}

			await Task.Yield();

			actual = ValueConverterProviders.Current;
			actual.ShouldNotBeNull();
			actual.ShouldBe(ValueConverterProviders.Default);
			actual.ShouldBeOfType<ValueConverterProvider>();
			actual.GetConverter().ShouldNotBe(converter);
		}
	}
}
