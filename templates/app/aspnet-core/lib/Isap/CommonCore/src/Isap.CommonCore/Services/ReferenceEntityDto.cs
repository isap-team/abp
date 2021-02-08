namespace Isap.CommonCore.Services
{
	public class ReferenceEntityDto<TPrimaryKey>: CommonEntityDto<TPrimaryKey>
	{
		public virtual string Title { get; set; }
		public virtual int DisplayOrder { get; set; }
	}
}
