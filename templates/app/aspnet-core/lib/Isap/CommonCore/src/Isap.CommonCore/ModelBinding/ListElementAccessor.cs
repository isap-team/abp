using System;
using System.Collections.Generic;
using Isap.Converters;

namespace Isap.CommonCore.ModelBinding
{
	public class ListElementAccessor<TElement>: IValueAccessor
		where TElement: new()
	{
		private readonly IListAdapter<TElement> _listAdapter = new ListAdapter<TElement>();
		private readonly IValueAccessor _componentValueAccessor;
		private readonly int _index;

		public ListElementAccessor(IValueAccessor componentValueAccessor, int index)
		{
			_componentValueAccessor = componentValueAccessor;
			_index = index;
		}

		public IValueConverter Converter => _componentValueAccessor.Converter;
		public Type ValueType => typeof(TElement);

		public object GetValue()
		{
			var list = (List<TElement>) _componentValueAccessor.GetValue();
			return list == null ? (object) null : _listAdapter.GetAt(list, _index);
		}

		public void SetValue(object value)
		{
			var list = (List<TElement>) _componentValueAccessor.EnsureValue();
			_listAdapter.SetAt(list, _index, (TElement) value);
		}

		public object EnsureValue()
		{
			object result = GetValue();
			if (result == null)
			{
				result = new TElement();
				SetValue(result);
			}

			return result;
		}
	}

	public static class ListElementAccessor
	{
		public static IValueAccessor Create(Type elementType, IValueAccessor componentValueAccessor, int index)
		{
			return (IValueAccessor) Activator.CreateInstance(typeof(ListElementAccessor<>).MakeGenericType(elementType), componentValueAccessor, index);
		}
	}
}
