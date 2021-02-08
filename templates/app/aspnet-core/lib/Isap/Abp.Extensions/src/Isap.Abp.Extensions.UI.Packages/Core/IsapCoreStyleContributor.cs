using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Isap.Abp.Extensions.UI.Packages.Core
{
	public class IsapCoreStyleContributor: BundleContributor
	{
		public override void ConfigureBundle(BundleConfigurationContext context)
		{
			context.Files.AddIfNotContains("/libs/isap/core/isap.css");
		}
	}
}
