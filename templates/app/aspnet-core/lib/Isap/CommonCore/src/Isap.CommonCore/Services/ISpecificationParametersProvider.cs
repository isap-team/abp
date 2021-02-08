namespace Isap.CommonCore.Services
{
	public interface ISpecificationParametersProvider
	{
		TParams GetParameters<TParams>();
	}
}
