using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Security.Claims;

namespace Isap.Abp.BackgroundJobs
{
	public abstract class ExtendedAsyncBackgroundJobBase<TArgs, TResult>: DomainServiceBase, IExtendedAsyncBackgroundJob<TArgs>
	{
		protected ICurrentPrincipalAccessor CurrentPrincipalAccessor => LazyGetRequiredService<ICurrentPrincipalAccessor>();

		async Task<object> IExtendedAsyncBackgroundJob<TArgs>.ExecuteAsync(TArgs args, CancellationToken cancellationToken)
		{
			return await ExecuteAsync(args, cancellationToken);
		}

		Task IAsyncBackgroundJob<TArgs>.ExecuteAsync(TArgs args) => ExecuteAsync(args, CancellationToken.None);

		public abstract Task<TResult> ExecuteAsync(TArgs args, CancellationToken cancellationToken);

		protected virtual async Task<TResult> ImpersonateAsync(string userName, Func<Task<TResult>> action)
		{
			List<Claim> claims = new List<Claim>
				{
					new Claim(AbpClaimTypes.TenantId, CurrentTenant.Id?.ToString()),
					//new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
					new Claim(AbpClaimTypes.UserName, userName),
					//user.UserRoles.ForEach(role => claims.Add(new Claim(AbpClaimTypes.Role, role)));
				};
			var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

			using (CurrentPrincipalAccessor.Change(principal))
				return await action();
		}
	}

	public abstract class ExtendedAsyncBackgroundJobBase<TArgs>: ExtendedAsyncBackgroundJobBase<TArgs, bool>
	{
	}
}
