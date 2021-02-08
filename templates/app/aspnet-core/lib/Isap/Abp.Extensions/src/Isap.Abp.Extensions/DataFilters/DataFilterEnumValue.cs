namespace Isap.Abp.Extensions.DataFilters
{
	public class DataFilterEnumValue
	{
		public DataFilterEnumValue(string id, string title)
		{
			Id = id;
			Title = title;
		}

		public DataFilterEnumValue()
		{
		}

		public string Id { get; set; }
		public string Title { get; set; }
	}
}
