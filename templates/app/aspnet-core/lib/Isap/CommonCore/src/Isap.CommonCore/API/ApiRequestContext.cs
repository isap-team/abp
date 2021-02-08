using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Isap.CommonCore.Extensions;
using Isap.Converters;
using JetBrains.Annotations;

namespace Isap.CommonCore.API
{
	public class ApiRequestContext
	{
		public ApiRequestContext()
		{
			Converter = ValueConverterProviders.Default.GetConverter();
			Cookies = new CookieContainer();
			Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public ApiRequestContext([NotNull] ApiRequestContext baseContext)
		{
			if (baseContext == null) throw new ArgumentNullException(nameof(baseContext));
			Converter = ValueConverterProviders.Default.GetConverter();
			Properties = new Dictionary<string, object>(baseContext.Properties, StringComparer.OrdinalIgnoreCase);
			Cookies = baseContext.Cookies;
		}

		public ApiRequestContext(ApiRequestContext baseContext, string apiLogin, string apiPassword)
			: this(baseContext)
		{
			ApiLogin = apiLogin;
			ApiPassword = apiPassword;
		}

		public ApiRequestContext(ApiRequestContext baseContext, string apiSubdomain, string apiLogin, string apiPassword)
			: this(baseContext)
		{
			ApiSubdomain = apiSubdomain;
			ApiLogin = apiLogin;
			ApiPassword = apiPassword;
		}

		public ApiRequestContext(ApiRequestContext baseContext, string apiSubdomain)
			: this(baseContext)
		{
			ApiSubdomain = apiSubdomain;
		}

		public IValueConverter Converter { get; set; }

		public CookieContainer Cookies { get; set; }

		public Dictionary<string, object> Properties { get; }

		public string ApiSubdomain
		{
			get => Converter.TryConvertTo<string>(Properties.GetOrDefault(nameof(ApiSubdomain))).AsDefaultIfNotSuccess();
			set => Properties[nameof(ApiSubdomain)] = value;
		}

		public string ApiLogin
		{
			get => Converter.TryConvertTo<string>(Properties.GetOrDefault(nameof(ApiLogin))).AsDefaultIfNotSuccess();
			set => Properties[nameof(ApiLogin)] = value;
		}

		public string ApiPassword
		{
			get => Converter.TryConvertTo<string>(Properties.GetOrDefault(nameof(ApiPassword))).AsDefaultIfNotSuccess();
			set => Properties[nameof(ApiPassword)] = value;
		}

		public CancellationToken CancellationToken
		{
			get => Converter.TryConvertTo<CancellationToken>(Properties.GetOrDefault(nameof(CancellationToken))).AsDefaultIfNotSuccess(CancellationToken.None);
			set => Properties[nameof(CancellationToken)] = value;
		}

		public ApiRequestBodyType RequestBodyType
		{
			get => Converter.TryConvertTo<ApiRequestBodyType>(Properties.GetOrDefault(nameof(RequestBodyType))).AsDefaultIfNotSuccess();
			set => Properties[nameof(RequestBodyType)] = value;
		}

		public bool ThrowIfError
		{
			get => Converter.TryConvertTo<bool>(Properties.GetOrDefault(nameof(ThrowIfError))).AsDefaultIfNotSuccess(true);
			set => Properties[nameof(ThrowIfError)] = value;
		}

		public bool RequiresAuthentication
		{
			get => Converter.TryConvertTo<bool>(Properties.GetOrDefault(nameof(RequiresAuthentication))).AsDefaultIfNotSuccess(true);
			set => Properties[nameof(RequiresAuthentication)] = value;
		}

		public TimeSpan RequestTimeout
		{
			get => Converter.TryConvertTo<TimeSpan>(Properties.GetOrDefault(nameof(RequestTimeout))).AsDefaultIfNotSuccess(ApiClientBase.DefaultRequestTimeout);
			set => Properties[nameof(RequestTimeout)] = value;
		}

		public bool WrapResponse
		{
			get => Converter.TryConvertTo<bool>(Properties.GetOrDefault(nameof(WrapResponse))).AsDefaultIfNotSuccess();
			set => Properties[nameof(WrapResponse)] = value;
		}

		public AuthTokenItem AuthTokenItem
		{
			get => Converter.TryConvertTo<AuthTokenItem>(Properties.GetOrDefault(nameof(AuthTokenItem))).AsDefaultIfNotSuccess();
			set => Properties[nameof(AuthTokenItem)] = value;
		}

		public bool IsAuthenticated => (AuthTokenItem?.ExpiresAt ?? DateTime.MinValue) >= DateTime.UtcNow;

		public object Tag { get; set; }
	}
}
