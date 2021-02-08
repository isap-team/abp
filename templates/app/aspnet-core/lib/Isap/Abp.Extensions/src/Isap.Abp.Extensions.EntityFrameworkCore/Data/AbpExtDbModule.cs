using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Isap.Abp.Extensions.Data
{
	public abstract class AbpExtDbModule<TModelBuilderIntf, TModelBuilderImpl>: AbpModule
		where TModelBuilderIntf: IAbpModelBuilder
		where TModelBuilderImpl: class, TModelBuilderIntf
	{
		protected bool IsMigrationMode { get; set; }

		protected abstract string DbProviderKey { get; }

		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			var serviceProvider = context.Services.BuildServiceProvider();
			IOptions<AbpExtDbOptions> options = serviceProvider.GetRequiredService<IOptions<AbpExtDbOptions>>();
			IsMigrationMode = options.Value.IsMigrationMode;
			SkipAutoServiceRegistration = options.Value.DataProviderKey != DbProviderKey;

			base.PreConfigureServices(context);
		}

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			base.ConfigureServices(context);

			if (!SkipAutoServiceRegistration)
			{
				context.Services.AddTransient(typeof(TModelBuilderIntf), typeof(TModelBuilderImpl));

				ConfigureDatabaseSpecificServices(context);

				Configure<AbpDbContextOptions>(options => ConfigureDbContextOptions(context, options));

				if (IsMigrationMode)
				{
					ConfigureMigrationsDbContext(context);
				}
				else
				{
					ConfigureMainDbContext(context);
				}
			}
		}

		protected virtual void ConfigureDatabaseSpecificServices(ServiceConfigurationContext context)
		{
		}

		protected abstract void ConfigureDbContextOptions(ServiceConfigurationContext context, AbpDbContextOptions options);
		protected abstract void ConfigureMainDbContext(ServiceConfigurationContext context);
		protected abstract void ConfigureMigrationsDbContext(ServiceConfigurationContext context);
	}
}
