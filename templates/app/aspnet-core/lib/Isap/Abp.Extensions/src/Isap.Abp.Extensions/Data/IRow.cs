namespace Isap.Abp.Extensions.Data
{
	public interface IRow<out TEntity>
	{
		int RowNumber { get; }
		TEntity Row { get; }
	}
}
