using System.Collections.Generic;

namespace Isap.CommonCore.Services
{
	public class QueryOptionsDto
	{
		public QueryOptionsDto(ICollection<DataFilterValueDto> filterValues, ICollection<SortOptionDto> sortOptions)
		{
			FilterValues = filterValues;
			SortOptions = sortOptions;
		}

		public QueryOptionsDto(ICollection<DataFilterValueDto> filterValues)
			: this(filterValues, null)
		{
		}

		public QueryOptionsDto(ICollection<SortOptionDto> sortOptions)
			: this(null, sortOptions)
		{
		}

		public QueryOptionsDto()
		{
		}

		public ICollection<DataFilterValueDto> FilterValues { get; set; }
		public ICollection<SortOptionDto> SortOptions { get; set; }
	}
}
