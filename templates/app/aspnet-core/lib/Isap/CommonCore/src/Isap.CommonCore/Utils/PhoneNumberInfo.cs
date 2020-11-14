namespace Isap.CommonCore.Utils
{
	/// <summary>
	///     Phone number parsed to country info and local number.
	/// </summary>
	public class PhoneNumberInfo
	{
		/// <summary>
		///     Counstructor.
		/// </summary>
		/// <param name="country">Country info.</param>
		/// <param name="localNumber">Local number.</param>
		public PhoneNumberInfo(CountryInfo country, string localNumber)
		{
			Country = country;
			LocalNumber = localNumber;
		}

		/// <summary>
		///     Country info.
		/// </summary>
		public CountryInfo Country { get; }

		/// <summary>
		///     Local phone number part.
		/// </summary>
		public string LocalNumber { get; }

		/// <summary>
		///     Full phone number including dial code.
		/// </summary>
		public string FullNumber => $"{Country.DialCode}{LocalNumber}";

		/// <summary>
		///     Formatted information about phone number.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Country.DialCode} {LocalNumber} ({Country.IsoCode}, {Country.Name})";
		}
	}
}
