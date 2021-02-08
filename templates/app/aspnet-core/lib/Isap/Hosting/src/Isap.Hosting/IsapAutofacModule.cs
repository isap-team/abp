using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using Isap.CommonCore;

namespace Isap.Hosting
{
	public class IsapAutofacModule: Module
	{
		protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(componentRegistry, registration);
			registration.PipelineBuilding += Registration_PipelineBuilding;
		}

		private void Registration_PipelineBuilding(object sender, IResolvePipelineBuilder e)
		{
			e.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (context, next) =>
				{
					if (context.Instance is ICommonInitialize initializable)
						initializable.Initialize();
					next(context);
				});
		}
	}
}
