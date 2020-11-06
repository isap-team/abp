using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Isap.CommonCore.DependencyInjection
{
	public class PropertyInjectContributeComponentModelConstruction: IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			foreach (var propertySet in model.Properties)
			{
				var attr = propertySet.Property.GetCustomAttributes<PropertyInjectAttribute>(true).SingleOrDefault();
				if (attr != null)
					propertySet.Dependency.IsOptional = attr.IsOptional;
			}
		}
	}
}
