using System;
using System.Collections.Generic;
using System.Linq;
using Isap.Abp.Extensions.Logging;
using Isap.Abp.Extensions.MultiTenancy;
using Isap.Abp.Extensions.Web;
using Isap.CommonCore.Integrations;
using Isap.CommonCore.Web.Middlewares.RequestLogging;
using Isap.CommonCore.Web.Middlewares.Tracing;
using Isap.Converters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MyCompanyName.MyProjectName.MultiTenancy;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.OAuth;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using Volo.Abp.TenantManagement.Web;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable UnusedParameter.Local

namespace MyCompanyName.MyProjectName.PublicGateway
{
	[DependsOn(
		typeof(MyProjectNameHttpApiModule),
		typeof(MyProjectNameHttpApiClientModule),
		typeof(IsapAbpExtensionsWebModule),
		typeof(AbpAspNetCoreAuthenticationOAuthModule),
		typeof(AbpAspNetCoreMvcClientModule),
		typeof(AbpAspNetCoreMvcUiBasicThemeModule),
		typeof(AbpAutofacModule),
		typeof(AbpCachingStackExchangeRedisModule),
		typeof(AbpFeatureManagementWebModule),
		typeof(AbpHttpClientIdentityModelWebModule),
		typeof(AbpIdentityWebModule),
		typeof(AbpTenantManagementWebModule),
		typeof(AbpAspNetCoreSerilogModule)
	)]
	public class MyProjectNamePublicGatewayModule: AbpModule
	{
		private const string DefaultCorsPolicyName = "Default";

		public override void ConfigureServices(ServiceConfigurationContext context)
		{
			var hostingEnvironment = context.Services.GetHostingEnvironment();
			var configuration = context.Services.GetConfiguration();

			IValueConverter converter = ValueConverterProviders.Default.GetConverter();
			context.Services.AddSingleton(converter);

			ConfigureRequestLogging(context, converter, configuration);
			ConfigureForwardedHeaders(context);
			ConfigureMultiTenancy();
			ConfigureCors(context, configuration);
			ConfigureAuthentication(context, converter, configuration);
			ConfigureSwaggerServices(context);
			ConfigureOcelot(context);
			ConfigureCache(configuration);
			ConfigureRedis(context, configuration, hostingEnvironment);
		}

		private void ConfigureRequestLogging(ServiceConfigurationContext context, IValueConverter converter, IConfiguration configuration)
		{
			context.Services.UseMsToCastleLoggingAdapter();

			Configure<IsapRequestLoggingOptions>(options =>
				{
					IConfigValueProvider config = new ConfigurationSectionValueProvider(converter, configuration.GetSection("RequestLogging"));

					options.IsEnabled = config.GetValue("IsEnabled", false);

					List<string> basePaths = config.GetValue("BasePaths", () => new List<string>());
					options.AddBasePaths(basePaths.ToArray());
				});
		}

		private void ConfigureForwardedHeaders(ServiceConfigurationContext context)
		{
			context.Services.Configure<ForwardedHeadersOptions>(options =>
				{
					options.ForwardedHeaders = ForwardedHeaders.All;
					options.KnownProxies.Clear();
					options.KnownNetworks.Clear();
				});
		}

		private void ConfigureMultiTenancy()
		{
			Configure<AbpMultiTenancyOptions>(options =>
				{
					options.IsEnabled = MultiTenancyConsts.IsEnabled;
				});

			if (MultiTenancyConsts.IsEnabled)
			{
				/*
				Configure<AbpTenantResolveOptions>(options =>
					{
						options.TenantResolvers.Add(new DefaultTenantResolveContributor());
					});
				*/

				Configure<AbpDefaultTenantStoreOptions>(options =>
					{
						options.Tenants = MultiTenancyConsts.DefaultTenants;
					});

				Configure<AbpAspNetCoreMultiTenancyOptions>(options =>
					{
						options.TenantKey = IsapMultiTenancyConsts.TenantHeaderName;
					});
			}
		}

		private void ConfigureAuthentication(ServiceConfigurationContext context, IValueConverter converter, IConfiguration configuration)
		{
			IdentityModelEventSource.ShowPII = true;

			context.Services
				.AddAuthentication("Bearer")
				.AddIdentityServerAuthentication(options =>
					{
						options.Authority = configuration["AuthServer:Authority"];
						options.ApiName = configuration["AuthServer:ApiName"];
						options.RequireHttpsMetadata =
							converter.TryConvertTo<bool>(configuration["AuthServer:RequireHttpsMetadata"]).AsDefaultIfNotSuccess(true);
					})
				;
			/*
			Configure<AbpClaimsMapOptions>(options =>
				{
					//options.Maps.Add(JwtClaimTypes.Subject, () => AbpClaimTypes.UserId);
					//options.Maps.Add(JwtClaimTypes.PreferredUserName, () => AbpClaimTypes.UserName);
					//options.Maps.Add(JwtClaimTypes.Email, () => AbpClaimTypes.Email);
					options.Maps.Add(JwtClaimTypes.EmailVerified, () => AbpClaimTypes.EmailVerified);
					options.Maps.Add(JwtClaimTypes.PhoneNumber, () => AbpClaimTypes.PhoneNumber);
					options.Maps.Add(JwtClaimTypes.PhoneNumberVerified, () => AbpClaimTypes.PhoneNumberVerified);
				});
			*/
		}

		private void ConfigureSwaggerServices(ServiceConfigurationContext context)
		{
			context.Services.AddSwaggerGen(
				options =>
					{
						options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProjectName API", Version = "v1" });
						options.DocInclusionPredicate((docName, description) => true);
						options.CustomSchemaIds(type => type.FullName);
					}
			);
		}

		private static void ConfigureOcelot(ServiceConfigurationContext context)
		{
			context.Services.AddOcelot(context.Services.GetConfiguration());
		}

		private void ConfigureCache(IConfiguration configuration)
		{
			Configure<AbpDistributedCacheOptions>(options =>
				{
					options.KeyPrefix = "MyProjectName:";
				});
		}

		private void ConfigureRedis(
			ServiceConfigurationContext context,
			IConfiguration configuration,
			IWebHostEnvironment hostingEnvironment)
		{
			/*
			context.Services.AddStackExchangeRedisCache(options =>
				{
					options.Configuration = configuration["Redis:Configuration"];
				});
			*/

			if (!hostingEnvironment.IsDevelopment())
			{
				var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
				context.Services
					.AddDataProtection()
					.PersistKeysToStackExchangeRedis(redis, "MyProjectName-Protection-Keys");
			}
		}

		private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
		{
			context.Services.AddCors(options =>
				{
					options.AddPolicy(DefaultCorsPolicyName, builder =>
						{
							builder
								.WithOrigins(
									configuration["App:CorsOrigins"]
										.Split(",", StringSplitOptions.RemoveEmptyEntries)
										.Select(o => o.RemovePostFix("/"))
										.ToArray()
								)
								.WithAbpExposedHeaders()
								.SetIsOriginAllowedToAllowWildcardSubdomains()
								.AllowAnyHeader()
								.AllowAnyMethod()
								.AllowCredentials();
						});
				});
		}

		public override void OnApplicationInitialization(ApplicationInitializationContext context)
		{
			var app = context.GetApplicationBuilder();
			// var env = context.GetEnvironment();

			app.UseMiddleware<LoggingTraceIdentifierMiddleware>();
			app.UseRequestResponseLogging();

			app.UseHttpMethodOverride();
			app.UseForwardedHeaders();

			app.UseCorrelationId();
			app.UseVirtualFiles();
			app.UseRouting();
			app.UseCors(DefaultCorsPolicyName);
			app.UseAuthentication();
			app.UseAbpClaimsMap();
			if (MultiTenancyConsts.IsEnabled)
			{
				app.UseMultiTenancy();
			}

			app.UseSwagger();
			app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProjectName API");
				});

			app.MapWhen(
				ctx => ctx.Request.Path.ToString().StartsWith("/api/abp/") ||
					ctx.Request.Path.ToString().StartsWith("/Abp/"),
				app2 =>
					{
						app2.UseRouting();
						app2.UseConfiguredEndpoints();
					}
			);

			app.UseOcelot().Wait();
		}
	}
}
