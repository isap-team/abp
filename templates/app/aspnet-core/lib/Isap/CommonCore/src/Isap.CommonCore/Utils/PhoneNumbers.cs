using System;
using System.Text.RegularExpressions;

namespace Isap.CommonCore.Utils
{
	/// <summary>
	///     Phone number utility.
	/// </summary>
	public static class PhoneNumbers
	{
		private static readonly Regex _phoneNumberLikeRegex =
			new Regex(@"^\s*\+?\s*[0-9]+\s*(\([0-9]+\))?\s*([0-9]+)((\s+|(\s*\-\s*))[0-9]+)*\s*$",
				RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex _nonSignificantCharsRegex = new Regex(@"[^\+\d]", RegexOptions.Compiled | RegexOptions.Singleline);

		/// <summary>
		///     Checks that value is phone number like.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsPhoneNumberLike(string value)
		{
			return _phoneNumberLikeRegex.IsMatch(value);
		}

		/// <summary>
		///     Romove non significant characters from phone number.
		/// </summary>
		/// <param name="phoneNumber">Input phone number.</param>
		/// <returns>Output phone number in format like +79151234567.</returns>
		public static string RemoveNonSignificantChars(string phoneNumber)
		{
			return _nonSignificantCharsRegex.Replace(phoneNumber, String.Empty);
		}

		/// <summary>
		///     Try parse phone number to country info and local number.
		/// </summary>
		/// <param name="phoneNumber">Phone number.</param>
		/// <returns>Parsed phone number info.</returns>
		public static PhoneNumberInfo TryGetPhoneNumberInfo(string phoneNumber)
		{
			phoneNumber = RemoveNonSignificantChars(phoneNumber);
			for (int i = Math.Min(phoneNumber.Length, CountryCodes.MaxDialCodeLength); i >= 2; i--)
			{
				if (CountryCodes.TryGetCountyInfoByDialCode(phoneNumber.Substring(0, i), out CountryInfo def))
					return new PhoneNumberInfo(def, phoneNumber.Substring(i));
			}

			return null;
		}

		/// <summary>
		///     Parse phone number to country info and local number.
		/// </summary>
		/// <param name="phoneNumber">Phone number.</param>
		/// <returns>Parsed phone number info.</returns>
		public static PhoneNumberInfo GetPhoneNumberInfo(string phoneNumber)
		{
			PhoneNumberInfo result = TryGetPhoneNumberInfo(phoneNumber);
			if (result == null)
				throw new InvalidOperationException($"Can't find country for phone number '{phoneNumber}'");
			return result;
		}

		/// <summary>
		///     Convert phone number from country internal number format to international number using country dial plans.
		/// </summary>
		/// <param name="phoneNumber">Internal number.</param>
		/// <param name="countryIsoCode">Country ISO code.</param>
		/// <param name="internationalNumber">International number result.</param>
		/// <returns>True if successfully converted.</returns>
		public static bool TryConvertToInternationalNumber(string phoneNumber, string countryIsoCode, out string internationalNumber)
		{
			phoneNumber = RemoveNonSignificantChars(phoneNumber ?? string.Empty);

			if (phoneNumber.Length == 0)
			{
				internationalNumber = null;
				return false;
			}

			if (phoneNumber[0] == '+')
			{
				PhoneNumberInfo phoneNumberInfo = TryGetPhoneNumberInfo(phoneNumber);
				if (phoneNumberInfo != null)
				{
					internationalNumber = phoneNumberInfo.FullNumber;
					return true;
				}
			}
			else
			{
				CountryDialPlans dialPlans = CountryDialPlans.TryGetCountryDialPlans(countryIsoCode);
				if (dialPlans != null)
				{
					foreach (DialPlan dialPlan in dialPlans.DialPlans)
					{
						if (phoneNumber.Length < dialPlan.InCountryCallNumberLength)
							continue;

						if (phoneNumber.Length == dialPlan.InCountryCallNumberLength)
						{
							if (CountryCodes.TryGetCountyInfoByIsoCode(countryIsoCode, out var country))
							{
								internationalNumber = country.DialCode + phoneNumber;
								return true;
							}

							continue;
						}

						if (!string.IsNullOrEmpty(dialPlan.CrossCountryCallNumberPrefix) && phoneNumber.StartsWith(dialPlan.CrossCountryCallNumberPrefix))
						{
							PhoneNumberInfo phoneNumberInfo = TryGetPhoneNumberInfo("+" + phoneNumber.Substring(dialPlan.CrossCountryCallNumberPrefix.Length));
							if (phoneNumberInfo != null)
							{
								internationalNumber = phoneNumberInfo.FullNumber;
								return true;
							}
						}
						else if (!string.IsNullOrEmpty(dialPlan.InCountryCallNumberPrefix) && phoneNumber.StartsWith(dialPlan.InCountryCallNumberPrefix))
						{
							if (CountryCodes.TryGetCountyInfoByIsoCode(countryIsoCode, out var country))
							{
								PhoneNumberInfo phoneNumberInfo =
									TryGetPhoneNumberInfo(country.DialCode + phoneNumber.Substring(dialPlan.InCountryCallNumberPrefix.Length));
								if (phoneNumberInfo != null)
								{
									internationalNumber = phoneNumberInfo.FullNumber;
									return true;
								}
							}
						}
					}
				}
			}

			internationalNumber = null;
			return false;
		}
	}
}
