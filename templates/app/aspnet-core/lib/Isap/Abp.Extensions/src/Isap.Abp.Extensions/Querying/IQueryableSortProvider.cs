using System.Linq;

namespace Isap.Abp.Extensions.Querying
{
	public interface IQueryableSortProvider
	{
		IQueryable Apply(IQueryable query);
	}

	public interface IQueryableSortProvider<TImpl>: IQueryableSortProvider
	{
		IQueryable<TImpl> Apply(IQueryable<TImpl> query);
	}
}
