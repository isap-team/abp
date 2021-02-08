using System;
using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.MultiTenancy
{
    public static class MultiTenancyConsts
    {
        public static class DefaultTenant
        {
            public static readonly Guid Id = new Guid("841a2213-885a-4c91-bc27-ea38f627414b");
            public static readonly string Name = "Default";
        }

        /* Enable/disable multi-tenancy easily in a single point.
         * If you will never need to multi-tenancy, you can remove
         * related modules and code parts, including this file.
         */
        public const bool IsEnabled = true;

        public static readonly TenantConfiguration[] DefaultTenants =
            {
                new TenantConfiguration(DefaultTenant.Id, DefaultTenant.Name),
            };
    }
}
