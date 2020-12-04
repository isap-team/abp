using System.Net.Http;
using System.Text;
using Isap.CommonCore.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Http;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.Authentication;
using Volo.Abp.Http.Client.DynamicProxying;
using Volo.Abp.Http.Modeling;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Tracing;

namespace Isap.Abp.Extensions
{
	public class ApmDynamicHttpProxyInterceptor<TService>: DynamicHttpProxyInterceptor<TService>
	{
		public ApmDynamicHttpProxyInterceptor(IDynamicProxyHttpClientFactory httpClientFactory, IOptions<AbpHttpClientOptions> clientOptions,
			IOptionsSnapshot<AbpRemoteServiceOptions> remoteServiceOptions, IApiDescriptionFinder apiDescriptionFinder, IJsonSerializer jsonSerializer,
			IRemoteServiceHttpClientAuthenticator clientAuthenticator, ICancellationTokenProvider cancellationTokenProvider,
			ICorrelationIdProvider correlationIdProvider, IOptions<AbpCorrelationIdOptions> correlationIdOptions, ICurrentTenant currentTenant)
			: base(httpClientFactory, clientOptions, remoteServiceOptions, apiDescriptionFinder, jsonSerializer, clientAuthenticator, cancellationTokenProvider,
				correlationIdProvider, correlationIdOptions, currentTenant)
		{
		}

		protected override void AddHeaders(IAbpMethodInvocation invocation, ActionApiDescriptionModel action, HttpRequestMessage requestMessage,
			ApiVersionInfo apiVersion)
		{
			base.AddHeaders(invocation, action, requestMessage, apiVersion);

			if (requestMessage.Method == HttpMethod.Post)
			{
				if (requestMessage.Content == null)
				{
					if (invocation.Method?.DeclaringType?.IsGenericType ?? false)
					{
						// К сожалению необязательные параметры обрабатываются некорректно через request body при POST запросах.
						if (invocation.Method.Name == "GetPage"
							&& invocation.Method.DeclaringType.GetGenericTypeDefinition() == typeof(IReferenceAppService<,>)
						)
						{
							requestMessage.Content = new StringContent(JsonConvert.SerializeObject(new QueryOptionsDto()), Encoding.UTF8,
								MimeTypes.Application.Json);
						}
					}
				}
			}
		}
	}
}
