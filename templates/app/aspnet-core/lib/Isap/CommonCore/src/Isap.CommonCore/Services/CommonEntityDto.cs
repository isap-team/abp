namespace Isap.CommonCore.Services
{
	public abstract class CommonEntityDto<TPrimaryKey>: ICommonEntityDto<TPrimaryKey>, ICommonEntity<TPrimaryKey>
	{
		public virtual TPrimaryKey Id { get; set; }

		object ICommonEntity.GetId() => Id;
	}
}
