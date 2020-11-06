using System;

namespace Isap.CommonCore.Utils
{
	public static class PhoneNumberHelper
	{
		private static bool TryNormalizePhoneNumber(string phone, string countryIsoCode, out string result)
		{
			return PhoneNumbers.TryConvertToInternationalNumber(phone, countryIsoCode, out result);
		}

		public static string TryNormalizePhoneNumber(string phone, string countryIsoCode)
		{
			phone = phone ?? string.Empty;
			string[] phoneParts = phone.Split(new[] { '#' }, 2);
			switch (phoneParts.Length)
			{
				case 1:
					phone = phoneParts[0];
					break;
				case 2:
					phone = phoneParts[1];
					break;
				default:
					throw new InvalidOperationException();
			}

			phone = PhoneNumbers.RemoveNonSignificantChars(phone);

			return TryNormalizePhoneNumber(phone, countryIsoCode, out string result) ? result : phone;
		}
	}
}
