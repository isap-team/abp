using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.CommonCore.Services;
using Volo.Abp.Settings;

namespace Isap.Abp.Extensions.Settings
{
	public interface ISettingAppService: ICommonAppService
	{
		Task<string> GetOrNullAsync(string name, string providerName, string providerKey);
		Task<List<SettingValue>> GetAllAsync(string[] names, string providerName, string providerKey);
	}
}
