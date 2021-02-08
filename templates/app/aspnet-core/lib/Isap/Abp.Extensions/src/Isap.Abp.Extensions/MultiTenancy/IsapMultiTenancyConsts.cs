using System;

namespace Isap.Abp.Extensions.MultiTenancy
{
	public static class IsapMultiTenancyConsts
	{
		public const string TenantHeaderName = "__tenant";
		public const bool IsEnabled = true;

		public static class DefaultTenant
		{
			public static readonly Guid Id = new Guid("841a2213-885a-4c91-bc27-ea38f627414b");
			public static readonly string Name = "Default";
		}
	}
}
