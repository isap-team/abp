namespace Isap.Abp.Extensions.Data.Specifications.OrderSpecs
{
	public class OrderSpecificationParameters
	{
		public OrderSpecificationParameters(bool isDescending = false)
		{
			IsDescending = isDescending;
		}

		public bool IsDescending { get; set; }
	}
}
