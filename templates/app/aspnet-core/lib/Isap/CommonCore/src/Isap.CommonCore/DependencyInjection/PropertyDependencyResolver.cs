using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Windsor;

namespace Isap.CommonCore.DependencyInjection
{
	public class PropertyDependencyResolver: ISubDependencyResolver
	{
		private readonly IWindsorContainer _container;

		public PropertyDependencyResolver(IWindsorContainer container)
		{
			_container = container;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			DependencyModel dependency)
		{
			if (dependency is PropertyDependencyModel propDependency)
			{
				var attr = propDependency.Property.GetAttribute<PropertyInjectAttribute>();
				if (attr != null)
					return _container.Kernel.HasDependency(attr.DependencyKey, propDependency.TargetType);
			}

			return false;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			DependencyModel dependency)
		{
			var propDependency = (PropertyDependencyModel) dependency;
			var attr = propDependency.Property.GetAttribute<PropertyInjectAttribute>();
			return _container.ResolveDependency(attr.DependencyKey, dependency.TargetType);
		}
	}
}
