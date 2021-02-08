using System;
using System.Collections.Generic;
using System.Linq;

namespace Isap.CommonCore.Utils
{
	public class DialPlan
	{
		public DialPlan(int inCountryCallNumberLength,
			string inCountryCallNumberPrefix, string crossCountryCallNumberPrefix,
			DateTime usedFrom, DateTime usedTo)
		{
			InCountryCallNumberPrefix = inCountryCallNumberPrefix;
			CrossCountryCallNumberPrefix = crossCountryCallNumberPrefix;
			InCountryCallNumberLength = inCountryCallNumberLength;
			UsedFrom = usedFrom;
			UsedTo = usedTo;
		}

		public DialPlan(int inCountryCallNumberLength, string inCountryCallNumberPrefix, string crossCountryCallNumberPrefix, DateTime usedFrom)
			: this(inCountryCallNumberLength, inCountryCallNumberPrefix, crossCountryCallNumberPrefix, usedFrom, DateTime.MaxValue)
		{
		}

		public DialPlan(int inCountryCallNumberLength, string inCountryCallNumberPrefix, string crossCountryCallNumberPrefix)
			: this(inCountryCallNumberLength, inCountryCallNumberPrefix, crossCountryCallNumberPrefix, DateTime.MinValue, DateTime.MaxValue)
		{
		}

		public int InCountryCallNumberLength { get; set; }
		public string InCountryCallNumberPrefix { get; }
		public string CrossCountryCallNumberPrefix { get; }
		public DateTime UsedFrom { get; }
		public DateTime UsedTo { get; }
	}

	public class CountryDialPlans
	{
		private static readonly List<CountryDialPlans> __countryDialPlans = new List<CountryDialPlans>
			{
				new CountryDialPlans("RU",
					new DialPlan(10, "8", "810", DateTime.MinValue, DateTime.Parse("2020-01-01")),
					new DialPlan(10, "0", "00", DateTime.Parse("2020-01-01")),
					new DialPlan(10, "7", null, DateTime.MinValue, DateTime.MinValue)
				),
			};

		private static readonly Dictionary<string, CountryDialPlans> __countryDialPlanMap = __countryDialPlans
			.ToDictionary(p => p.CountryIsoCode, StringComparer.OrdinalIgnoreCase);

		public CountryDialPlans(string countryIsoCode, params DialPlan[] dialPlans)
		{
			CountryIsoCode = countryIsoCode;
			DialPlans = dialPlans;
		}

		public string CountryIsoCode { get; }
		public DialPlan[] DialPlans { get; }

		public static CountryDialPlans TryGetCountryDialPlans(string countryIsoCode)
		{
			return __countryDialPlanMap.TryGetValue(countryIsoCode, out CountryDialPlans result) ? result : null;
		}

		public DialPlan GetDielPlan(DateTime dateTime)
		{
			return DialPlans.FirstOrDefault(p => p.UsedFrom <= dateTime && dateTime < p.UsedTo);
		}
	}
}
