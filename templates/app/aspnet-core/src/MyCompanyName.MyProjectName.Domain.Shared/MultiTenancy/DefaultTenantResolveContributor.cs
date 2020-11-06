using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.MultiTenancy
{
	public class DefaultTenantResolveContributor: TenantResolveContributorBase
	{
		public const string ContributorName = "Default";

		public override string Name => ContributorName;

		public override void Resolve(ITenantResolveContext context)
		{
			context.TenantIdOrName = MultiTenancyConsts.DefaultTenant.Id.ToString();
		}
	}
}
