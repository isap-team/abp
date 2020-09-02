using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Isap.CommonCore;

namespace Isap.Hosting
{
	public class IsapAutofacModule: Module
	{
		protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(componentRegistry, registration);
			registration.Activated += OnActivated;
		}

		private void OnActivated(object sender, ActivatedEventArgs<object> e)
		{
			if (e.Instance is ICommonInitialize initializable)
				initializable.Initialize();
		}
	}
}
