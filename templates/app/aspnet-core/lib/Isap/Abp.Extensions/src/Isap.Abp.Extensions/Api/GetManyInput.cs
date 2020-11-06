namespace Isap.Abp.Extensions.Api
{
	public class GetManyInput<TKey>
	{
		public GetManyInput(TKey[] idList)
		{
			IdList = idList;
		}

		public GetManyInput()
			: this(new TKey[0])
		{
		}

		public TKey[] IdList { get; set; }
	}
}
