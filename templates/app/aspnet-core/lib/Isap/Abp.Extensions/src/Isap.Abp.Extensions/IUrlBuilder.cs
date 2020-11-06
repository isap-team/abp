using System;
using System.Collections.Generic;

namespace Isap.Abp.Extensions
{
	public interface IUrlBuilder
	{
		Uri BuildAbsoluteUrl(Uri url, bool isInternal = false);
		Uri BuildAbsoluteUrl(string url, bool isInternal = false);

		Uri BuildControllerUrl(string controller, string action, Dictionary<string, object> queryString, bool isInternal = false);
		Uri BuildControllerUrl(string controller, string action, object queryString, bool isInternal = false);

		Uri BuildAppServiceUrl(string controller, string action, Dictionary<string, object> queryString, bool isInternal = false);
		Uri BuildAppServiceUrl(string controller, string action, object queryString, bool isInternal = false);

		Uri BuildFileUrl(string fileStorageProviderName, Uri url, bool isInternal = false);
		Uri BuildFileUrl(string fileStorageProviderName, string url, bool isInternal = false);
		Uri BuildFileUrl(Uri url, bool isInternal = false);
		Uri BuildFileUrl(string url, bool isInternal = false);

		Uri BuildPrivateAreaUrl(Uri url, bool isInternal = false);
		Uri BuildPrivateAreaUrl(string url, bool isInternal = false);
	}
}
