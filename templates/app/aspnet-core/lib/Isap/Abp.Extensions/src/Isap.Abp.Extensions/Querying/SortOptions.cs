using System.Collections.Generic;

namespace Isap.Abp.Extensions.Querying
{
	public static class SortOptions
	{
		public static List<SortOption> Create(string fieldName, bool isDescending = false)
		{
			return new List<SortOption>
				{
					new SortOption(fieldName, isDescending),
				};
		}

		public static List<SortOption> Then(this List<SortOption> options, string fieldName, bool isDescending = false)
		{
			options.Add(new SortOption(fieldName, isDescending));
			return options;
		}
	}
}
