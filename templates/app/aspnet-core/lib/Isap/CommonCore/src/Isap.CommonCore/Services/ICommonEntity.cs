namespace Isap.CommonCore.Services
{
	public interface ICommonEntity
	{
		object GetId();
	}

	public interface ICommonEntity<out TPrimaryKey>: ICommonEntity
	{
		/// <summary>
		///     Id of the entity.
		/// </summary>
		TPrimaryKey Id { get; }
	}
}
