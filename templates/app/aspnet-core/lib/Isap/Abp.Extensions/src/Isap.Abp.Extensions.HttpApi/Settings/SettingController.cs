using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Settings;

namespace Isap.Abp.Extensions.Settings
{
	[RemoteService]
	[Route("api/system/setting")]
	public class SettingController: IsapControllerBase, ISettingAppService
	{
		protected ISettingAppService SettingAppService => LazyGetRequiredService<ISettingAppService>();

		[HttpGet]
		[Route("get-or-null")]
		public async Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
		{
			return await SettingAppService.GetOrNullAsync(name, providerName, providerKey);
		}

		[HttpGet]
		[Route("get-all")]
		public async Task<List<SettingValue>> GetAllAsync(string[] names, string providerName, string providerKey)
		{
			return await SettingAppService.GetAllAsync(names, providerName, providerKey);
		}
	}
}
