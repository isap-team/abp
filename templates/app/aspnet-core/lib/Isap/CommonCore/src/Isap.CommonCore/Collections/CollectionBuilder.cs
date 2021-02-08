using System;
using System.Collections;
using System.Collections.Generic;

namespace Isap.CommonCore.Collections
{
	public interface ICollectionBuilder
	{
		ICollectionBuilder Add(object item);
		object ToArray();
		ICollection ToCollection();
		object ToGenericCollection();
		IList ToList();
		object ToGenericList();
	}

	public interface ICollectionBuilder<T>: ICollectionBuilder
	{
		ICollectionBuilder<T> Add(T item);
		new T[] ToArray();
		new ICollection<T> ToGenericCollection();
		new IList<T> ToGenericList();
	}

	public class CollectionBuilder<T>: ICollectionBuilder<T>
	{
		private readonly List<T> _items = new List<T>();

		#region Implementation of ICollectionBuilder

		public ICollectionBuilder<T> Add(T item)
		{
			_items.Add(item);
			return this;
		}

		public T[] ToArray()
		{
			return _items.ToArray();
		}

		public ICollection<T> ToGenericCollection()
		{
			return _items;
		}

		public IList<T> ToGenericList()
		{
			return _items;
		}

		public IList ToList()
		{
			return _items;
		}

		object ICollectionBuilder.ToGenericList()
		{
			return ToGenericList();
		}

		public ICollection ToCollection()
		{
			return _items;
		}

		object ICollectionBuilder.ToGenericCollection()
		{
			return ToGenericCollection();
		}

		ICollectionBuilder ICollectionBuilder.Add(object item)
		{
			return Add((T) item);
		}

		object ICollectionBuilder.ToArray()
		{
			return ToArray();
		}

		#endregion
	}

	public static class CollectionBuilder
	{
		public static ICollectionBuilder Create(Type elementType)
		{
			return (ICollectionBuilder) Activator.CreateInstance(typeof(CollectionBuilder<>).MakeGenericType(elementType));
		}
	}
}
