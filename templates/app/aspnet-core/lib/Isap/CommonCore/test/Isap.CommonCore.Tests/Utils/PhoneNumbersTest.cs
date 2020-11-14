using Isap.CommonCore.Utils;
using Shouldly;
using Xunit;

namespace Isap.CommonCore.Tests.Utils
{
	public class PhoneNumbersTest
	{
		[Theory]
		[InlineData("+79151234567", "+79151234567")]
		[InlineData("+7 (915) 123-45-67", "+79151234567")]
		public void RemoveNonSignificantCharsTest(string input, string expectedResult)
		{
			string actual = PhoneNumbers.RemoveNonSignificantChars(input);
			actual.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData("+79151234567", "RU")]
		[InlineData("+619151234567", "AU")]
		public void TryGetPhoneNumberInfoSuccessTest(string phoneNumber, string expectedCountryIsoCode)
		{
			PhoneNumberInfo actual = PhoneNumbers.TryGetPhoneNumberInfo(phoneNumber);
			actual.ShouldNotBeNull();
			actual.Country.ShouldNotBeNull();
			actual.Country.IsoCode.ShouldBe(expectedCountryIsoCode);
		}

		[Theory]
		[InlineData("+79151234567", "RU", "+79151234567")]
		[InlineData("79151234567", "RU", "+79151234567")]
		[InlineData("89151234567", "RU", "+79151234567")]
		[InlineData("810619151234567", "RU", "+619151234567")]
		[InlineData("+7 (0", "RU", "+70")]
		public void TryConvertToInternationalNumberSuccessTest(string phoneNumber, string countryIsoCode, string exprectedInternationalNumber)
		{
			PhoneNumbers.TryConvertToInternationalNumber(phoneNumber, countryIsoCode, out string actualInternationalNumber).ShouldBeTrue();
			actualInternationalNumber.ShouldBe(exprectedInternationalNumber);
		}

		[Theory]
		[InlineData("8 (0", "RU")]
		public void TryConvertToInternationalNumberFailTest(string phoneNumber, string countryIsoCode)
		{
			PhoneNumbers.TryConvertToInternationalNumber(phoneNumber, countryIsoCode, out string actualInternationalNumber).ShouldBeFalse();
			actualInternationalNumber.ShouldBeNull();
		}
	}
}
