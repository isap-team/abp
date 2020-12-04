using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Services;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Isap.Abp.Extensions.Settings
{
	public class SettingAppService: AppServiceBase, ISettingAppService
	{
		protected ISettingManagementStore SettingStore => LazyGetRequiredService<ISettingManagementStore>();

		public async Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
		{
			return await SettingStore.GetOrNullAsync(name, providerName, providerKey);
		}

		public async Task<List<SettingValue>> GetAllAsync(string[] names, string providerName, string providerKey)
		{
			if (names == null)
				return await SettingStore.GetListAsync(providerName, providerKey);
			return await SettingStore.GetListAsync(names, providerName, providerKey);
		}
	}
}
