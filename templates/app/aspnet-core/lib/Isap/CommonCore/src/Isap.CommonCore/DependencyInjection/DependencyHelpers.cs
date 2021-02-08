using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Isap.CommonCore.DependencyInjection
{
	public static class DependencyHelpers
	{
		public static string GetDependencyKey(string key, Type service)
		{
			return $"{service.FullName}#{key}";
		}

		public static string GetDependencyKey<TService>(string key)
		{
			return GetDependencyKey(key, typeof(TService));
		}

		public static ComponentRegistration<TService> NamedDependency<TService>(this ComponentRegistration<TService> registration,
			string dependencyKey)
			where TService: class
		{
			return registration.Named(GetDependencyKey<TService>(dependencyKey));
		}

		public static bool HasDependency(this IKernel kernel, string dependencyKey, Type service)
		{
			return dependencyKey == null
					? kernel.HasComponent(service)
					: kernel.HasComponent(GetDependencyKey(dependencyKey, service))
				;
		}

		public static bool HasDependency<TService>(this IKernel kernel, string dependencyKey)
		{
			return kernel.HasDependency(dependencyKey, typeof(TService));
		}

		public static object ResolveDependency(this IKernel kernel, string dependencyKey, Type service)
		{
			return dependencyKey == null
					? kernel.Resolve(service)
					: kernel.Resolve(GetDependencyKey(dependencyKey, service), service)
				;
		}

		public static TService ResolveDependency<TService>(this IKernel kernel, string dependencyKey)
		{
			return (TService) kernel.ResolveDependency(dependencyKey, typeof(TService));
		}

		public static object ResolveDependency(this IWindsorContainer container, string dependencyKey, Type service)
		{
			return container.Kernel.ResolveDependency(dependencyKey, service);
		}

		public static TService ResolveDependency<TService>(this IWindsorContainer container, string dependencyKey)
		{
			return container.Kernel.ResolveDependency<TService>(dependencyKey);
		}
	}
}
