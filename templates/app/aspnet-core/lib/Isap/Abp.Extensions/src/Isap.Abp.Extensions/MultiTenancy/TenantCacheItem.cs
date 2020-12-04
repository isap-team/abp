using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.MultiTenancy
{
	public interface ITenantBase: ICommonEntity<Guid>
	{
		string Name { get; }
		Guid UnregisteredUserId { get; }
	}

	public class TenantCacheItem: ICommonEntityDto<Guid>, ITenantBase
	{
		public TenantCacheItem()
		{
		}

		public TenantCacheItem(Guid id, string name, Guid unregisteredUserId)
		{
			Id = id;
			Name = name;
			UnregisteredUserId = unregisteredUserId;
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
		public Guid UnregisteredUserId { get; }

		object ICommonEntity.GetId() => Id;
	}
}
