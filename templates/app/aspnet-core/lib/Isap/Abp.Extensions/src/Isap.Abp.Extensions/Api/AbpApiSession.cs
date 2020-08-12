using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using JetBrains.Annotations;

namespace Isap.Abp.Extensions.Api
{
	public interface IAbpApiSession
	{
		/// <summary>
		///     Идентификатор тенанта.
		/// </summary>
		string TenantId { get; }

		/// <summary>
		///     Идентификатор пользователя.
		/// </summary>
		string UserId { get; }

		/// <summary>
		///     Ключ элемента кластера для распределения нагрузки.
		/// </summary>
		int? NodeKey { get; }

		bool IsAuthenticated { get; }
		AuthToken AuthToken { get; }

		[NotNull]
		Collection<Cookie> Cookies { get; }

		IDictionary<string, string> RequestHeaders { get; }

		IAbpApiSessionBuilder GetBuilder();
	}

	public interface IAbpApiSessionBuilder
	{
		void Assign(DateTime now, int? tenantId, IAbpAuthResponse authResponse);
		void Assign(DateTime now, Guid? tenantId, Guid userId, int? nodeKey = null);
		void Assign(DateTime now, string tenantId, string userId, int? nodeKey = null);

		//void Assign(ExternalAccountDto externalAccount);
	}

	public class AbpApiSession: IAbpApiSession, IAbpApiSessionBuilder
	{
		public AbpApiSession()
		{
			Cookies = new Collection<Cookie>();
			RequestHeaders = new Dictionary<string, string>();
		}

		public AbpApiSession(int nodeKey)
			: this()
		{
			NodeKey = nodeKey;
		}

		#region Implementation of IAbpApiSessionBuilder

		public void Assign(DateTime now, int? tenantId, IAbpAuthResponse authResponse)
		{
			var authToken = new AuthToken(authResponse.AccessToken, authResponse.EncryptedAccessToken, now.AddSeconds(authResponse.ExpireInSeconds));
			Assign(now, authToken, tenantId?.ToString(), authResponse.UserId.ToString(), authResponse.NodeKey);
		}

		public void Assign(DateTime now, Guid? tenantId, Guid userId, int? nodeKey = null)
		{
			Assign(now, null, tenantId?.ToString(), userId.ToString(), nodeKey);
		}

		public void Assign(DateTime now, string tenantId, string userId, int? nodeKey = null)
		{
			Assign(now, null, tenantId, userId, nodeKey);
		}

		#endregion

		#region Implementation of IAbpApiSession

		/// <summary>
		///     Идентификатор тенанта.
		/// </summary>
		public string TenantId { get; private set; }

		/// <summary>
		///     Идентификатор пользователя в базе коммуникатора.
		/// </summary>
		public string UserId { get; private set; }

		/// <summary>
		///     Ключ элемента кластера для распределения нагрузки.
		/// </summary>
		public int? NodeKey { get; private set; }

		public bool IsAuthenticated { get; private set; }

		public AuthToken AuthToken { get; private set; }

		public Collection<Cookie> Cookies { get; }

		public IDictionary<string, string> RequestHeaders { get; }

		public IAbpApiSessionBuilder GetBuilder() => this;

		#endregion

		protected virtual void Assign(DateTime now, AuthToken authToken, string tenantId, string userId, int? nodeKey = null)
		{
			AuthToken = authToken;
			TenantId = tenantId;
			UserId = userId;
			NodeKey = nodeKey;
			IsAuthenticated = AuthToken != null && AuthToken.ExpireAt > DateTime.Now || !string.IsNullOrEmpty(userId);
		}
	}
}
