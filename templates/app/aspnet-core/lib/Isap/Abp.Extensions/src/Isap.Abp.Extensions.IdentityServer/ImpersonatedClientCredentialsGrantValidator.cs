using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Isap.Converters;
using Volo.Abp.Uow;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Isap.Abp.Extensions.IdentityServer
{
	public class ImpersonatedClientCredentialsGrantValidator: IsapGrantValidatorBase
	{
		public override string GrantType => IsapGrantTypes.ImpersonatedClientCredentials;

		protected override async Task InternalValidateAsync(ExtensionGrantValidationContext context)
		{
			string userId = context.Request.Raw.Get("userId");
			string userName = context.Request.Raw.Get("userName");

			using (var uow = UnitOfWorkManager.Begin())
			{
				IdentityUser user = null;
				if (!string.IsNullOrEmpty(userId))
				{
					ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(userId);
					if (!attempt.IsSuccess)
					{
						context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_user_id_format");
						return;
					}

					user = await UserClaimsPrincipalFactory.UserManager.FindByIdAsync(userId);
				}

				else if (!string.IsNullOrEmpty(userName))
				{
					user = await UserClaimsPrincipalFactory.UserManager.FindByNameAsync(userName);
				}

				context.Result = await CreateGrantValidationResult(user, IsapAuthenticationMethods.Impersonation);
				await uow.CompleteAsync();
			}
		}
	}
}
