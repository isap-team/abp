using System;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.MultiTenancy
{
	public interface ITenantBase: ICommonEntity<Guid>
	{
		string Name { get; }
	}

	public class TenantCacheItem: ICommonEntityDto<Guid>, ITenantBase
	{
		public TenantCacheItem()
		{
		}

		public TenantCacheItem(Guid id, string name)
		{
			Id = id;
			Name = name;
		}

		public Guid Id { get; set; }
		public string Name { get; set; }

		object ICommonEntity.GetId() => Id;
	}
}
