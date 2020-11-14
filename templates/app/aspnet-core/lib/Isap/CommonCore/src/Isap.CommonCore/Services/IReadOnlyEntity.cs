namespace Isap.CommonCore.Services
{
	public interface IReadOnlyEntity<out TPrimaryKey>
	{
		TPrimaryKey GetId();
	}
}
