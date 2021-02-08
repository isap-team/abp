using System;
using System.Collections.Generic;
using System.Linq;
using Isap.CommonCore.Extensions;

namespace Isap.CommonCore.Utils
{
	/// <summary>
	///     Country codes utility.
	/// </summary>
	public static class CountryCodes
	{
		private static readonly List<CountryInfo> __definitions = typeof(CountryCodes).ReadEmbeddedResourceAsJsonObject<List<CountryInfo>>("CountryCodes.json");

		private static readonly Dictionary<string, CountryInfo> __dialCodeToCountyMap = __definitions
			.GroupBy(def => def.DialCode)
			.ToDictionary(g => g.Key, g => g.First());

		private static readonly Dictionary<string, CountryInfo> __isoCodeToCountryMap = __definitions
			.ToDictionary(d => d.IsoCode, StringComparer.OrdinalIgnoreCase);

		/// <summary>
		///     Maximum dial code length including '+' sign.
		/// </summary>
		public static int MaxDialCodeLength { get; } = __definitions.Max(def => def.DialCode.Length);

		/// <summary>
		///     Try get country info by the specified dial code.
		/// </summary>
		/// <param name="dialCode">Dial code including '+' sign.</param>
		/// <param name="countryInfo">Found country info.</param>
		/// <returns>True if country info is successfully found.</returns>
		public static bool TryGetCountyInfoByDialCode(string dialCode, out CountryInfo countryInfo)
		{
			return __dialCodeToCountyMap.TryGetValue(dialCode, out countryInfo);
		}

		/// <summary>
		///     Try get country info by the specified ISO code.
		/// </summary>
		/// <param name="isoCode">ISO code.</param>
		/// <param name="countryInfo">Found country info.</param>
		/// <returns>True if country info is successfully found.</returns>
		public static bool TryGetCountyInfoByIsoCode(string isoCode, out CountryInfo countryInfo)
		{
			return __isoCodeToCountryMap.TryGetValue(isoCode, out countryInfo);
		}
	}
}
