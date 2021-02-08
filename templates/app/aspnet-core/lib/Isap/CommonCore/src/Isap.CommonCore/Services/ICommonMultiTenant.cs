namespace Isap.CommonCore.Services
{
	public interface ICommonMultiTenant<TKey>
	{
		TKey TenantId { get; set; }
	}
}
