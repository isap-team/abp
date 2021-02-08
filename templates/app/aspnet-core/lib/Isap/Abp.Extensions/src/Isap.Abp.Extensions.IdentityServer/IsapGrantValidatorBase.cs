using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Isap.Abp.Extensions.Domain;
using Isap.Converters;
using Microsoft.AspNetCore.Authentication;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Isap.Abp.Extensions.IdentityServer
{
	public abstract class IsapGrantValidatorBase: IExtensionGrantValidator, ISupportsLazyServices
	{
		public IServiceProvider ServiceProvider { protected get; set; }

		protected readonly object ServiceProviderLock = new object();
		object ISupportsLazyServices.ServiceProviderLock => ServiceProviderLock;

		IServiceProvider ISupportsLazyServices.ServiceProvider => ServiceProvider;
		ConcurrentDictionary<Type, object> ISupportsLazyServices.ServiceReferenceMap { get; } = new ConcurrentDictionary<Type, object>();

		protected ISystemClock Clock => this.LazyGetRequiredService<ISystemClock>();
		protected IValueConverter Converter => this.LazyGetRequiredService<IValueConverter>();
		protected IUnitOfWorkManager UnitOfWorkManager => this.LazyGetRequiredService<IUnitOfWorkManager>();
		protected AbpUserClaimsPrincipalFactory UserClaimsPrincipalFactory => this.LazyGetRequiredService<AbpUserClaimsPrincipalFactory>();
		protected ICurrentTenant CurrentTenant => this.LazyGetRequiredService<ICurrentTenant>();

		public abstract string GrantType { get; }

		public async Task ValidateAsync(ExtensionGrantValidationContext context)
		{
			string tenantIdStr = context.Request.Raw.Get("tenantId");
			if (!string.IsNullOrEmpty(tenantIdStr))
			{
				ConvertAttempt<Guid> attempt = Converter.TryConvertTo<Guid>(tenantIdStr);
				if (!attempt.IsSuccess)
				{
					context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_tenant_id_format");
					return;
				}

				Guid? tenantId = attempt.Result;

				using (CurrentTenant.Change(tenantId))
					await InternalValidateAsync(context);
			}
			else
				await InternalValidateAsync(context);
		}

		protected abstract Task InternalValidateAsync(ExtensionGrantValidationContext context);

		protected virtual async Task<GrantValidationResult> CreateGrantValidationResult(IdentityUser user, string authenticationMethod,
			Action<List<Claim>> includeAdditionalClaims = default, Dictionary<string, object> customResponse = default)
		{
			if (user == null)
				return new GrantValidationResult(TokenRequestErrors.InvalidGrant, "user_not_found", customResponse);

			ClaimsPrincipal principal = await UserClaimsPrincipalFactory.CreateAsync(user);
			var time = new DateTimeOffset(Clock.UtcNow.UtcDateTime).ToUnixTimeSeconds().ToString();

			var claims = new List<Claim>(principal.Claims)
				{
					new Claim(JwtClaimTypes.IdentityProvider, IdentityServerConstants.LocalIdentityProvider),
					new Claim(JwtClaimTypes.AuthenticationTime, time, ClaimValueTypes.Integer64),
					new Claim(JwtClaimTypes.AuthenticationMethod, authenticationMethod),
				};
			includeAdditionalClaims?.Invoke(claims);
			return new GrantValidationResult(new ClaimsPrincipal(new ClaimsIdentity(claims)), customResponse);
		}

		protected virtual async Task<GrantValidationResult> CreateGrantValidationResult(Guid? userId, string authenticationMethod,
			Action<List<Claim>> includeAdditionalClaims = default, Dictionary<string, object> customResponse = default)
		{
			if (!userId.HasValue)
				return new GrantValidationResult(TokenRequestErrors.InvalidGrant, "user_not_found");

			IdentityUser user = await UserClaimsPrincipalFactory.UserManager.FindByIdAsync(userId.Value.ToString());
			return await CreateGrantValidationResult(user, authenticationMethod, includeAdditionalClaims, customResponse);
		}
	}
}
