using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Castle.Core.Internal;

namespace Isap.CommonCore.Utils
{
	public class FullName
	{
		private static readonly Regex __splitRegex = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex __validNameRegex = new Regex(@"^[\w]+['-]?[\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex __validNameForSearchRegex = new Regex(@"^[_%\w]+['-]?[_%\w]+$", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex __invalidCharsRegex = new Regex(@"[_%\d]", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex __invalidCharsForSearchRegex = new Regex(@"[\d]", RegexOptions.Compiled | RegexOptions.Singleline);

		public FullName(string lastName, string firstName, string middleName)
		{
			FirstName = firstName;
			MiddleName = middleName;
			LastName = lastName;
			ShortName = ToShortName(lastName, firstName, middleName);
		}

		public FullName()
		{
		}

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string ShortName { get; set; }

		public string ToShortName(FullName fullName)
			=> ToShortName(fullName.LastName, fullName.FirstName, fullName.MiddleName);

		public string ToShortName(string lastName, string firstName, string middleName)
		{
			string result = String.Empty;
			if (!lastName.IsNullOrEmpty())
				result = lastName;

			if (!firstName.IsNullOrEmpty())
			{
				result = string.Format("{0} {1}.", result, firstName[0]);

				if (!middleName.IsNullOrEmpty())
					result = string.Format("{0} {1}.", result, middleName[0]);
			}

			return result;
		}

		public override string ToString()
		{
			return string.Join(" ", new[] { LastName, FirstName, MiddleName }.Where(name => !string.IsNullOrWhiteSpace(name)));
		}

		public static bool TryParseForSearch(string value, out FullName result)
		{
			return TryParseForSearch(value, true, out result);
		}

		public static bool TryParseForSearch(string value, bool isLastNameInHead, out FullName result)
		{
			result = null;
			if (string.IsNullOrWhiteSpace(value)) return false;
			if (__invalidCharsForSearchRegex.IsMatch(value)) return false;

			if (!value.EndsWith("%"))
				value = value + "%";

			List<string> items = __splitRegex.Split(value, 4)
				.Where(item => !string.IsNullOrEmpty(item))
				.Where(item => item != "%")
				.ToList();

			if (items.Any(item => !__validNameForSearchRegex.IsMatch(item))) return false;

			switch (items.Count)
			{
				case 1:
					result = isLastNameInHead ? new FullName(items[0], null, null) : new FullName(null, items[0], null);
					return true;
				case 2:
					result = isLastNameInHead ? new FullName(items[0], items[1], null) : new FullName(items[1], items[0], null);
					return true;
				case 3:
					result = isLastNameInHead ? new FullName(items[0], items[1], items[2]) : new FullName(items[2], items[0], items[1]);
					return true;
				default:
					return false;
			}
		}

		public static FullName TryParseForSearch(string value)
		{
			return TryParseForSearch(value, out FullName fullName) ? fullName : null;
		}

		public static bool TryParse(string value, out FullName result)
		{
			return TryParse(value, true, out result);
		}

		public static bool TryParse(string value, bool isLastNameInHead, out FullName result)
		{
			result = null;
			if (string.IsNullOrWhiteSpace(value)) return false;
			if (__invalidCharsRegex.IsMatch(value)) return false;

			List<string> items = __splitRegex.Split(value).Where(item => !string.IsNullOrEmpty(item)).ToList();
			if (items.Any(item => !__validNameRegex.IsMatch(item))) return false;
			switch (items.Count)
			{
				case 1:
					result = isLastNameInHead ? new FullName(items[0], null, null) : new FullName(null, items[0], null);
					return true;
				case 2:
					result = isLastNameInHead ? new FullName(items[0], items[1], null) : new FullName(items[1], items[0], null);
					return true;
				case 3:
					result = isLastNameInHead ? new FullName(items[0], items[1], items[2]) : new FullName(items[2], items[0], items[1]);
					return true;
				default:
					return false;
			}
		}

		public static FullName TryParse(string value)
		{
			return TryParse(value, out FullName fullName) ? fullName : null;
		}

		public static FullName Parse(string value)
		{
			return TryParse(value) ?? throw new InvalidOperationException($"Full name value is in invalid format '{value}'.");
		}
	}
}
