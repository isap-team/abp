using System;
using System.Threading.Tasks;
using Isap.Converters.Extensions;

namespace Isap.CommonCore.Services.InMemoryServices
{
	public abstract class InMemoryBasicAppServiceBase<TEntityDto, TKey>
		: InMemoryReferenceAppServiceBase<TEntityDto, TKey>, IBasicAppService<TEntityDto, TKey>
		where TEntityDto: class, ICommonEntityDto<TKey>
	{
		protected InMemoryBasicAppServiceBase(IInMemoryEntityCollectionProvider collectionProvider)
			: base(collectionProvider)
		{
		}

		protected InMemoryBasicAppServiceBase()
		{
		}

		public Task<TEntityDto> Create(TEntityDto entry)
		{
			TKey id = EnsureKey(entry);
			if (!Entries.TryAdd(id, entry))
				throw new InvalidOperationException();
			return Task.FromResult(entry);
		}

		public Task<TEntityDto> Update(TEntityDto entry)
		{
			TKey id = entry.Id;
			if (!Entries.TryGetValue(id, out var currentEntry))
				throw new InvalidOperationException();
			if (!Entries.TryUpdate(id, entry, currentEntry))
				throw new InvalidOperationException();
			return Task.FromResult(entry);
		}

		public async Task<TEntityDto> Save(TEntityDto entry)
		{
			if (entry.Id.IsDefaultValue())
				return await Create(entry);
			return await Update(entry);
		}

		public Task Delete(TKey id)
		{
			if (typeof(ICommonSoftDelete).IsAssignableFrom(typeof(TEntityDto)))
			{
				if (Entries.TryGetValue(id, out var entry))
				{
					// ReSharper disable once SuspiciousTypeConversion.Global
					((ICommonSoftDelete) entry).IsDeleted = true;
				}
			}
			else
				Entries.TryRemove(id, out _);

			return Task.CompletedTask;
		}

		public Task<TEntityDto> Undelete(TKey id)
		{
			if (!typeof(ICommonSoftDelete).IsAssignableFrom(typeof(TEntityDto)))
				throw new NotSupportedException($"Entity '{typeof(TEntityDto)}' is not supports soft deletion.");

			if (!Entries.TryGetValue(id, out var entry))
				return Task.FromResult<TEntityDto>(null);
			// ReSharper disable once SuspiciousTypeConversion.Global
			((ICommonSoftDelete) entry).IsDeleted = false;
			return Task.FromResult(entry);
		}

		protected virtual TKey EnsureKey(TEntityDto entry)
		{
			if (entry.Id.IsDefaultValue())
				entry.Id = GetNewKey();
			return entry.Id;
		}

		protected abstract TKey GetNewKey();
	}

	public abstract class InMemoryBasicAppServiceBase<TEntityDto>: InMemoryBasicAppServiceBase<TEntityDto, Guid>
		where TEntityDto: class, ICommonEntityDto<Guid>
	{
		protected InMemoryBasicAppServiceBase(IInMemoryEntityCollectionProvider collectionProvider)
			: base(collectionProvider)
		{
		}

		protected InMemoryBasicAppServiceBase()
		{
		}

		protected override Guid GetNewKey() => Guid.NewGuid();
	}
}
