using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.MultiTenancy
{
    public class DefaultTenantResolveContributor: TenantResolveContributorBase
    {
        public const string ContributorName = "Default";

        public override string Name => ContributorName;

        public override Task ResolveAsync(ITenantResolveContext context)
        {
            context.TenantIdOrName = MultiTenancyConsts.DefaultTenant.Id.ToString();
            return Task.CompletedTask;
        }
    }
}
