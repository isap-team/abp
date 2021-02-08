namespace Isap.Abp.Extensions.Querying
{
	public class SortOption
	{
		public string FieldName;
		public bool IsDescending;

		public SortOption(string fieldName, bool isDescending = false)
		{
			FieldName = fieldName;
			IsDescending = isDescending;
		}

		public SortOption()
		{
		}
	}
}
