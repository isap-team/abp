using System;
using System.Reflection;
using Isap.Abp.Extensions.Clustering;
using Isap.Abp.Extensions.Data;
using Isap.Abp.Extensions.Data.Specifications;
using Isap.Abp.Extensions.DataFilters;
using Isap.Abp.Extensions.DataFilters.Converters;
using Isap.Abp.Extensions.Localization;
using Isap.Abp.Extensions.Metadata;
using Isap.Abp.Extensions.SystemServices;
using Isap.Abp.Extensions.Validation;
using Isap.Abp.Extensions.Workflow;
using Isap.CommonCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Timing;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

namespace Isap.Abp.Extensions
{
	[DependsOn(
		typeof(AbpSettingManagementDomainModule),
		typeof(AbpDddApplicationModule),
		typeof(AbpAutoMapperModule),
		typeof(AbpCachingModule),
		typeof(AbpTimingModule),
		typeof(AbpIdentityDomainModule),
		typeof(AbpTenantManagementDomainModule)
	)]
	public class IsapAbpExtensionsModule: AbpModule
	{
		private IDisposable _clockControl;

		public override void PreConfigureServices(ServiceConfigurationContext context)
		{
			IConfiguration config = context.Services.GetConfiguration();
			Configure<AbpClusterNodeOptions>(config.GetSection("ClusterNode"));

			context.Services.AddSingleton(MetadataProvider.Instance);
			context.Services.AddSingleton<IMetadataProvider>(x => x.GetRequiredService<MetadataProvider>());
			context.Services.AddSingleton<IMetadataRegistrar>(x => x.GetRequiredService<MetadataProvider>());

			context.Services.AddSingleton(new SpecificationBuilderRepository());
			context.Services.AddSingleton<ISpecificationBuilderRepository>(x => x.GetRequiredService<SpecificationBuilderRepository>());
			context.Services.AddSingleton<ISpecificationBuilderRepositoryRegistrar>(x => x.GetRequiredService<SpecificationBuilderRepository>());

			context.Services.AddConventionalRegistrar(new IsapAbpConventionalRegistrar());
		}

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			base.ConfigureServices(context);

			context.Services.AddTransient(x => ClockProvider.Current);

			Configure<AbpVirtualFileSystemOptions>(options =>
				{
					options.FileSets.AddEmbedded<IsapAbpExtensionsModule>();
				});

			Configure<AbpLocalizationOptions>(options =>
				{
					options.Resources
						.Add<AbpExtensionsResource>("ru")
						.AddVirtualJson("/Localization/AbpExtensions")
						;
				});

			ObjectAccessor<IServiceProvider> serviceProviderAccessor = context.Services.GetSingletonInstance<ObjectAccessor<IServiceProvider>>();

			Configure<AbpAutoMapperOptions>(options =>
				{
					options.AddMaps<IsapAbpExtensionsModule>(validate: true);
					options.Configurators.Add(ctx =>
						{
							ctx.MapperConfiguration.ConstructServicesUsing(type => serviceProviderAccessor.Value.GetService(type));
						});
				});

			context.Services.Replace(ServiceDescriptor.Transient<IMethodInvocationValidator, IsapMethodInvocationValidator>());

			Assembly thisAssembly = GetType().Assembly;

			context.Services.AddSingleton(new ShutdownCancellationTokenProvider());
			context.Services.AddSingleton<IShutdownCancellationTokenProvider>(x => x.GetRequiredService<ShutdownCancellationTokenProvider>());
			context.Services.AddSingleton<IShutdownCancellationTokenManager>(x => x.GetRequiredService<ShutdownCancellationTokenProvider>());

			context.Services.AddSingleton(new DataFilterValueConverterFactory(serviceProviderAccessor));
			context.Services.AddSingleton<IDataFilterValueConverterFactory>(x => x.GetRequiredService<DataFilterValueConverterFactory>());
			context.Services.AddSingleton<IDataFilterValueConverterFactoryBuilder>(x => x.GetRequiredService<DataFilterValueConverterFactory>());

			context.Services.GetSingletonInstance<DataFilterValueConverterFactory>().RegisterProducts(thisAssembly, context.Services);

			context.Services.AddSingleton(new CustomPredicateBuilderFactory(serviceProviderAccessor));
			context.Services.AddSingleton<ICustomPredicateBuilderFactory>(x => x.GetRequiredService<CustomPredicateBuilderFactory>());
			context.Services.AddSingleton<ICustomPredicateBuilderFactoryBuilder>(x => x.GetRequiredService<CustomPredicateBuilderFactory>());

			context.Services.GetSingletonInstance<CustomPredicateBuilderFactory>().RegisterProducts(thisAssembly, context.Services);

			context.Services.AddTransient<IIdGenerator<Guid>, GuidIdGenerator>();

			context.Services.AddSingleton<EntityDataStore>();
			context.Services.AddSingleton<IEntityDataStore>(x => x.GetRequiredService<EntityDataStore>());
			context.Services.AddSingleton<IEntityDataStoreBuilder>(x => x.GetRequiredService<EntityDataStore>());

			ObjectAccessor<IDataFilterDataStore> dataFilterDataStoreAccessor = context.Services.AddObjectAccessor<IDataFilterDataStore>();

			context.Services.AddSingleton(x =>
				{
					var dataStore = new DataFilterDataStore
						{
							DataFilterProvider = x.GetRequiredService<IDataFilterProvider>(),
						};
					dataFilterDataStoreAccessor.Value = dataStore;
					return dataStore;
				});
			context.Services.AddSingleton<IDataFilterDataStore>(x => x.GetRequiredService<DataFilterDataStore>());
			context.Services.AddSingleton<IDataFilterDataStoreBuilder>(x => x.GetRequiredService<DataFilterDataStore>());

			context.Services.AddSingleton(new DocumentWorkflowStateStrategyFactory(serviceProviderAccessor));
			context.Services.AddSingleton<IDocumentWorkflowStateStrategyFactory>(x => x.GetRequiredService<DocumentWorkflowStateStrategyFactory>());
			context.Services.AddSingleton<IDocumentWorkflowStateStrategyFactoryBuilder>(x => x.GetRequiredService<DocumentWorkflowStateStrategyFactory>());

			context.Services.AddSingleton(new DocumentWorkflowFactory(serviceProviderAccessor));
			context.Services.AddSingleton<IDocumentWorkflowFactory>(x => x.GetRequiredService<DocumentWorkflowFactory>());
			context.Services.AddSingleton<IDocumentWorkflowFactoryBuilder>(x => x.GetRequiredService<DocumentWorkflowFactory>());

			context.Services.GetSingletonInstance<DocumentWorkflowStateStrategyFactory>().RegisterProducts(thisAssembly, context.Services);
			context.Services.GetSingletonInstance<DocumentWorkflowFactory>().RegisterProducts(thisAssembly, context.Services);
		}

		public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
		{
			base.OnPreApplicationInitialization(context);

			_clockControl = ClockProvider.Use(context.ServiceProvider.GetRequiredService<IClock>());
		}

		public override void OnApplicationInitialization(ApplicationInitializationContext context)
		{
			base.OnApplicationInitialization(context);

			context.ServiceProvider.GetRequiredService<IEntityDataStoreBuilder>()
				.With(builder =>
					{
						builder.Add(AbpExtensionsMetadata.EntityDefs.EntityDef);
						builder.Add(AbpExtensionsMetadata.EntityDefs.DataFilterDef);
					});

			context.ServiceProvider.GetRequiredService<IDataFilterDataStoreBuilder>()
				.With(builder =>
					{
						builder.AddRange(AbpExtensionsMetadata.EntityDefs.EntityDef.DataFilters);
						builder.AddRange(AbpExtensionsMetadata.EntityDefs.DataFilterDef.DataFilters);
					});
		}

		public override void OnApplicationShutdown(ApplicationShutdownContext context)
		{
			_clockControl?.Dispose();
			_clockControl = null;

			context.ServiceProvider.GetRequiredService<IShutdownCancellationTokenManager>().EnterShutdownMode();

			base.OnApplicationShutdown(context);
		}
	}
}
