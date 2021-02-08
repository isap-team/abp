using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Collections
{
	public interface IInMemoryDataCollection<in TKey, out TEntity>
		where TEntity: ICommonEntity<TKey>
	{
		TEntity GetById(TKey id);
		IEnumerable<TEntity> GetAll();
		int GetCount();
	}

	public interface IInMemoryDataCollectionBuilder<in TKey, in TEntity>
		where TEntity: ICommonEntity<TKey>
	{
		void Add(TEntity entity);
		void AddRange(IEnumerable<TEntity> entities);
	}

	public class InMemoryDataCollection<TKey, TEntity>: IInMemoryDataCollection<TKey, TEntity>, IInMemoryDataCollectionBuilder<TKey, TEntity>
		where TEntity: ICommonEntity<TKey>
	{
		private readonly ConcurrentDictionary<TKey, TEntity> _values = new ConcurrentDictionary<TKey, TEntity>();

		public TEntity GetById(TKey id)
		{
			return _values.TryGetValue(id, out TEntity value) ? value : throw new InvalidOperationException();
		}

		public IEnumerable<TEntity> GetAll()
		{
			return _values.Values;
		}

		public int GetCount()
		{
			return _values.Count;
		}

		public void Add(TEntity entity)
		{
			_values.AddOrUpdate(entity.Id, entity, (id, e) => entity);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			if (entities != null)
				foreach (TEntity entity in entities)
					Add(entity);
		}
	}
}
