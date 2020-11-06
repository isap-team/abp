using System.Collections.Generic;
using System.Linq;

namespace Isap.CommonCore.ModelBinding
{
	public interface IListAdapter
	{
	}

	public interface IListAdapter<TElement>: IListAdapter
	{
		TElement GetAt(List<TElement> list, int index);
		void SetAt(List<TElement> list, int index, TElement value);
	}

	public class ListAdapter<TElement>: IListAdapter<TElement>
	{
		public TElement GetAt(List<TElement> list, int index)
		{
			return index < list.Count ? list[index] : default(TElement);
		}

		public void SetAt(List<TElement> list, int index, TElement value)
		{
			if (index >= list.Count)
				list.AddRange(Enumerable.Repeat(default(TElement), index + 1 - list.Count));
			list[index] = value;
		}
	}
}
