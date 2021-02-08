namespace Isap.Abp.Extensions.Data
{
	public interface IIdGenerator
	{
		object NextValue();
	}

	public interface IIdGenerator<out T>: IIdGenerator
	{
		new T NextValue();
	}
}
