using System.Collections;
using System.Collections.Generic;

namespace Isap.Abp.Extensions.Querying
{
	public interface IEnumerableSortProvider
	{
		IEnumerable Apply(IEnumerable query);
	}

	public interface IEnumerableSortProvider<TImpl>: IEnumerableSortProvider
	{
		IEnumerable<TImpl> Apply(IEnumerable<TImpl> query);
	}
}
