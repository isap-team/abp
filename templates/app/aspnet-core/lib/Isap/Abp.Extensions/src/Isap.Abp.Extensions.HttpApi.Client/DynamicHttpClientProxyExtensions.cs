using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.DynamicProxying;

namespace Isap.Abp.Extensions
{
	public static class DynamicHttpClientProxyExtensions
	{
		/// <summary>
		///     Registers HTTP Client Proxies for all public interfaces
		///     extend the <see cref="IRemoteService" /> interface in the
		///     given <paramref name="assembly" />.
		/// </summary>
		/// <param name="services">Service collection</param>
		/// <param name="assembly">The assembly containing the service interfaces</param>
		/// <param name="remoteServiceConfigurationName">
		///     The name of the remote service configuration to be used by the HTTP Client proxies.
		///     See <see cref="AbpRemoteServiceOptions" />.
		/// </param>
		/// <param name="asDefaultServices">
		///     True, to register the HTTP client proxy as the default implementation for the services.
		/// </param>
		public static IServiceCollection AddHttpClientProxies<TRemoteService>(
			[NotNull] this IServiceCollection services,
			[NotNull] Assembly assembly,
			[NotNull] string remoteServiceConfigurationName = RemoteServiceConfigurationDictionary.DefaultName,
			bool asDefaultServices = true)
		{
			Check.NotNull(services, nameof(assembly));

			//TODO: Make a configuration option and add remoteServiceName inside it!
			//TODO: Add option to change type filter

			var serviceTypes = assembly.GetTypes().Where(IsSuitableForDynamicClientProxying<TRemoteService>);

			foreach (var serviceType in serviceTypes)
			{
				services.AddHttpClientProxy(
					serviceType,
					remoteServiceConfigurationName,
					asDefaultServices
				);

				// TODO: К сожалению необязательные параметры обрабатываются некорректно через request body при POST запросах.
				// Следующий код надо будет удалить, если внесут соответсвующие изменения в инфраструктуру ABP.
				var abpInterceptorType = typeof(DynamicHttpProxyInterceptor<>).MakeGenericType(serviceType);
				var interceptorType = typeof(ApmDynamicHttpProxyInterceptor<>).MakeGenericType(serviceType);
				services.Replace(ServiceDescriptor.Transient(abpInterceptorType, interceptorType));
			}

			return services;
		}

		static bool IsSuitableForDynamicClientProxying<TRemoteService>(Type type)
		{
			//TODO: Add option to change type filter

			return type.IsInterface
				&& type.IsPublic
				&& !type.IsGenericType
				&& typeof(TRemoteService).IsAssignableFrom(type);
		}
	}
}
