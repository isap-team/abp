namespace Isap.Abp.Extensions.Domain
{
	public class DefaultEntityBuilder<TIntf, TImpl>: IEntityBuilder<TIntf, TImpl>
		where TImpl: new()
	{
		public TImpl CreateNew(TIntf source)
		{
			return new TImpl();
		}
	}
}
