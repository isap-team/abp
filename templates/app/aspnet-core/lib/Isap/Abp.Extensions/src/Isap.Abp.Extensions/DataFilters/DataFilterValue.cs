using System;

namespace Isap.Abp.Extensions.DataFilters
{
	public class DataFilterValue
	{
		public DataFilterValue(Guid dataFilterId, string values)
		{
			DataFilterId = dataFilterId;
			Values = values;
		}

		public DataFilterValue()
		{
		}

		public Guid DataFilterId { get; set; }
		public string Values { get; set; }
	}
}
