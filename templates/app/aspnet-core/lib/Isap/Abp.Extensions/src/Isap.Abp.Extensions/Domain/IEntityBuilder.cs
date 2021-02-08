namespace Isap.Abp.Extensions.Domain
{
	public interface IEntityBuilder<in TIntf, out TImpl>
	{
		TImpl CreateNew(TIntf source);
	}
}
