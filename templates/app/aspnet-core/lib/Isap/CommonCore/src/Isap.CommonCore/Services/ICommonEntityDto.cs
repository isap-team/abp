namespace Isap.CommonCore.Services
{
	public interface ICommonEntityDto<TPrimaryKey>
	{
		/// <summary>
		///     Id of the entity.
		/// </summary>
		TPrimaryKey Id { get; set; }
	}
}
