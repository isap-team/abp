using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Api
{
	public class GetPageInput
	{
		public GetPageInput(int pageNumber, int pageSize, bool countTotal, QueryOptionsDto queryOptions)
		{
			PageNumber = pageNumber;
			PageSize = pageSize;
			CountTotal = countTotal;
			QueryOptions = queryOptions;
		}

		public GetPageInput(int pageNumber, int pageSize)
			: this(pageNumber, pageSize, false, null)
		{
		}

		public GetPageInput()
			: this(1, 10, false, null)
		{
		}

		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public bool CountTotal { get; set; }
		public QueryOptionsDto QueryOptions { get; set; }
	}
}
