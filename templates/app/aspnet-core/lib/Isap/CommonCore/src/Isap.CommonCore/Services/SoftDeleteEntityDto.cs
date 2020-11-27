namespace Isap.CommonCore.Services
{
	public class SoftDeleteEntityDto<TKey>: CommonEntityDto<TKey>, ICommonSoftDelete
	{
		public bool IsDeleted { get; set; }
	}
}
