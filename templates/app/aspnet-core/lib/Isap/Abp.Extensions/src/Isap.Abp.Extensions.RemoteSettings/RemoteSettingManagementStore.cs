using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Settings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Isap.Abp.Extensions
{
	[Dependency(ReplaceServices = true)]
	public class RemoteSettingManagementStore: ISettingManagementStore, ITransientDependency
	{
		public ISettingAppService SettingAppService { get; set; }

		public async Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
		{
			return await SettingAppService.GetOrNullAsync(name, providerName, providerKey);
		}

		public async Task<List<SettingValue>> GetListAsync(string providerName, string providerKey)
		{
			return await SettingAppService.GetAllAsync(null, providerName, providerKey);
		}

		public async Task<List<SettingValue>> GetListAsync(string[] names, string providerName, string providerKey)
		{
			return await SettingAppService.GetAllAsync(names, providerName, providerKey);
		}

		public Task SetAsync(string name, string value, string providerName, string providerKey)
		{
			throw new NotSupportedException();
		}

		public Task DeleteAsync(string name, string providerName, string providerKey)
		{
			throw new NotSupportedException();
		}
	}
}
