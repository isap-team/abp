using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Isap.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Http;
using Volo.Abp.Json;
using Volo.Abp.Minify.Scripts;

namespace Isap.Abp.Extensions.Web.Controllers
{
	[Area("Isap")]
	[Route("Isap/AppSettingsScript")]
	[DisableAuditing]
	[RemoteService(false)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class IsapAppSettingsScriptController: AbpController
	{
		public const string AuthTokenSessionKey = ".AbpAuthToken";

		private readonly AbpExtWebOptions _webOptions;
		private readonly AbpAspNetCoreMultiTenancyOptions _multiTenancyOptions;
		private readonly AbpAspNetCoreMvcOptions _options;
		private readonly IJsonSerializer _jsonSerializer;
		private readonly IJavascriptMinifier _javascriptMinifier;

		public IsapAppSettingsScriptController(
			IOptions<AbpExtWebOptions> webOptions,
			IOptions<AbpAspNetCoreMultiTenancyOptions> multiTenancyOptions,
			IOptions<AbpAspNetCoreMvcOptions> options,
			IJsonSerializer jsonSerializer,
			IJavascriptMinifier javascriptMinifier)
		{
			_webOptions = webOptions.Value;
			_multiTenancyOptions = multiTenancyOptions.Value;
			_options = options.Value;
			_jsonSerializer = jsonSerializer;
			_javascriptMinifier = javascriptMinifier;
		}

		[HttpGet]
		[Produces(MimeTypes.Application.Javascript, MimeTypes.Text.Plain)]
		public async Task<IActionResult> Index()
		{
			Dictionary<string, string> headers = await PrepareHeaders();

			string script = @$"
var abp = abp || {{}};
(function () {{

    abp.appPath = '{(_webOptions.AbpScriptsBaseUrl ?? "/").TrimEnd('/') + "/"}';

    $.extend(abp.ajax, {{
        defaultOpts: {{
            headers: {_jsonSerializer.Serialize(headers, indented: true)}
        }}
    }});

}})();
";
			return Content(
				_options.MinifyGeneratedScript == true
					? _javascriptMinifier.Minify(script)
					: script,
				MimeTypes.Application.Javascript
			);
		}

		private async Task<Dictionary<string, string>> PrepareHeaders()
		{
			var headers = new Dictionary<string, string>();

			if (CurrentTenant.Id.HasValue)
				headers.Add(_multiTenancyOptions.TenantKey, CurrentTenant.Id.Value.ToString());

			string authHeader = HttpContext.Request.Headers["Authorization"].ToString();
			if (string.IsNullOrEmpty(authHeader) && CurrentUser.IsAuthenticated)
			{
				string authToken = HttpContext.Session.GetString(AuthTokenSessionKey);
				if (string.IsNullOrEmpty(authToken))
				{
					ConvertAttempt<string> attempt = await RequestAuthTokenAsync(CurrentTenant.Id, CurrentUser.Id, CurrentUser.UserName);
					if (!attempt.IsSuccess)
						throw new UserFriendlyException(attempt.Message);
					authToken = attempt.Result;
					HttpContext.Session.SetString(AuthTokenSessionKey, authToken);
				}

				authHeader = $"Bearer {authToken}";
			}

			if (!string.IsNullOrEmpty(authHeader))
				headers.Add("Authorization", authHeader);
			return headers;
		}

		private async Task<ConvertAttempt<string>> RequestAuthTokenAsync(Guid? tenantId, Guid? userId, string userName)
		{
			if (string.IsNullOrEmpty(_webOptions.AuthServer?.Authority))
				return ConvertAttempt.Fail<string>("Возникла ошибка авторизации API-клиента, свзязанная с отсутсвием на текущем сервере настроек сервера авторизации.");

			using (var client = new HttpClient())
			{
				DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(_webOptions.AuthServer.Authority);
				if (disco.IsError)
					return ConvertAttempt.Fail<string>(disco.Error);

				var parameters = new Dictionary<string, string>
					{
						{ OidcConstants.TokenRequest.GrantType, "impersonated_client_credentials" },
						{ OidcConstants.TokenRequest.Scope, $"{_webOptions.AuthServer.ApiName}" },
						{ OidcConstants.TokenRequest.ClientId, _webOptions.AuthServer.ClientId },
						{ OidcConstants.TokenRequest.ClientSecret, _webOptions.AuthServer.ClientSecret },
					};
				if (tenantId.HasValue)
					parameters.Add("tenantId", tenantId.Value.ToString());
				if (userId.HasValue)
					parameters.Add("userId", userId.Value.ToString());
				else
					parameters.Add("userName", userName);
				TokenResponse tokenResponse = await client.RequestTokenRawAsync(disco.TokenEndpoint, parameters);
				if (tokenResponse.IsError)
					return ConvertAttempt.Fail<string>(tokenResponse.Error);

				return ConvertAttempt.Success(tokenResponse.AccessToken);
			}
		}
	}
}
