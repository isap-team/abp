namespace Isap.CommonCore.Services
{
	public interface ICommonOwnedEntity<TKey>
	{
		TKey OwnerId { get; set; }
	}
}
