using Newtonsoft.Json;

namespace Isap.CommonCore.Utils
{
	/// <summary>
	///     Information about country.
	/// </summary>
	public class CountryInfo
	{
		/// <summary>
		///     Country name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		///     ISO code.
		/// </summary>
		[JsonProperty("code")]
		public string IsoCode { get; set; }

		/// <summary>
		///     Country dial code.
		/// </summary>
		[JsonProperty("dial_code")]
		public string DialCode { get; set; }
	}
}
