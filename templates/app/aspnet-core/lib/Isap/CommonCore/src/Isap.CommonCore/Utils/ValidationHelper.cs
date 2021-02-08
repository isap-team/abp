using System.Text.RegularExpressions;

namespace Isap.CommonCore.Utils
{
	public static class ValidationHelper
	{
		public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
		public const string PhoneRegex = @"^((\+7|7|8)+([0-9]){10})$";

		private static readonly Regex _cleanPhoneNumberRegex = new Regex(@"[^\+0-9]", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex _checkPhoneNumberRegex = new Regex(PhoneRegex, RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex _checkEmailRegex = new Regex(EmailRegex, RegexOptions.Compiled | RegexOptions.Singleline);

		public static bool IsEmail(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}

			return _checkEmailRegex.IsMatch(value);
		}

		public static bool IsPhone(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}

			value = _cleanPhoneNumberRegex.Replace(value, string.Empty);

			return _checkPhoneNumberRegex.IsMatch(value);
		}
	}
}
