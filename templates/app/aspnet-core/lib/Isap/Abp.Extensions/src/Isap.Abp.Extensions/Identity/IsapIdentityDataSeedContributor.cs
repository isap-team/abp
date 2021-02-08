using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Isap.Abp.Extensions.Data;
using Isap.CommonCore.Extensions;
using Isap.Converters;
using Isap.Converters.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Isap.Abp.Extensions.Identity
{
	public class IsapIdentityDataSeedContributor: IsapDataSeedContributorBase, ITransientDependency
	{
		private static readonly Dictionary<string, Action<RoleDto, XAttribute, IValueConverter>> _setPropertyActionMap =
			new Dictionary<string, Action<RoleDto, XAttribute, IValueConverter>>
				{
					{ nameof(RoleDto.Id), (role, attr, converter) => role.Id = converter.ConvertTo<Guid>(attr.Value) },
					{ nameof(RoleDto.Name), (role, attr, converter) => role.Name = attr.Value },
					{ nameof(RoleDto.IsDefault), (role, attr, converter) => role.IsDefault = converter.ConvertTo<bool>(attr.Value) },
					{ nameof(RoleDto.IsStatic), (role, attr, converter) => role.IsStatic = converter.ConvertTo<bool>(attr.Value) },
					{ nameof(RoleDto.IsPublic), (role, attr, converter) => role.IsPublic = converter.ConvertTo<bool>(attr.Value) },
					{ nameof(RoleDto.TenantId), (role, attr, converter) => role.TenantId = converter.TryConvertTo<Guid?>(attr.Value).AsDefaultIfNotSuccess() },
				};

		protected IValueConverter Converter => LazyServiceProvider.LazyGetRequiredService<IValueConverter>();
		protected IPermissionManager PermissionManager => LazyServiceProvider.LazyGetRequiredService<IPermissionManager>();
		//protected IPermissionGrantRepository PermissionManager => LazyServiceProvider.LazyGetRequiredService<IPermissionManager>();
		protected IPermissionDefinitionManager PermissionDefinitionManager => LazyServiceProvider.LazyGetRequiredService<IPermissionDefinitionManager>();
		protected IStringLocalizerFactory StringLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IStringLocalizerFactory>();
		protected IdentityRoleManager RoleManager => LazyServiceProvider.LazyGetRequiredService<IdentityRoleManager>();
		protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();

		public override async Task SeedAsync(DataSeedContext context)
		{
			DirectoryInfo appDataDir = GetAppDataDir(context);

			if (!appDataDir.Exists)
				throw new InvalidOperationException();

			DirectoryInfo rolesDir = appDataDir.GetDirectories("Roles").FirstOrDefault();
			if (rolesDir != null)
			{
				await ImportRoles(context, rolesDir);
			}
		}

		private async Task ImportRoles(DataSeedContext context, DirectoryInfo rolesDir)
		{
			foreach (FileInfo file in rolesDir.GetFiles("*.xml"))
			{
				RoleDto role = await ParseRole(file);
				await ImportRole(context, role);
			}
		}

		private async Task<IdentityRole> ImportRole(DataSeedContext context, RoleDto role)
		{
			if (role.TenantId != context.TenantId)
				throw new InvalidOperationException();

			IdentityRole identityRole = await RoleManager.FindByIdAsync(role.Id.ToString())
					?? await RoleManager.FindByNameAsync(role.Name)
				;

			if (identityRole == null)
			{
				identityRole = new IdentityRole(role.Id.IfDefault(() => GuidGenerator.Create()), role.Name, role.TenantId)
					{
						IsDefault = role.IsDefault,
						IsStatic = role.IsStatic,
						IsPublic = role.IsPublic,
					};
				(await RoleManager.CreateAsync(identityRole)).CheckErrors();
			}
			else if (identityRole.TenantId != role.TenantId)
			{
				Guid roleId = role.Id.IfDefault(() => GuidGenerator.Create());
				if (await RoleManager.FindByIdAsync(roleId.ToString()) != null)
					throw new InvalidOperationException();

				identityRole = new IdentityRole(roleId, role.Name, role.TenantId)
					{
						IsDefault = role.IsDefault,
						IsStatic = role.IsStatic,
						IsPublic = role.IsPublic,
					};
				(await RoleManager.CreateAsync(identityRole)).CheckErrors();
			}
			else
			{
				if (role.Id != Guid.Empty && role.Id != identityRole.Id)
					throw new InvalidOperationException();

				if (identityRole.Name != role.Name)
					identityRole.ChangeName(role.Name);

				identityRole.IsDefault = role.IsDefault;
				identityRole.IsStatic = role.IsStatic;
				identityRole.IsPublic = role.IsPublic;

				(await RoleManager.UpdateAsync(identityRole)).CheckErrors();
			}

			List<PermissionWithGrantedProviders> grantedPermissions = await PermissionManager.GetAllAsync(RolePermissionValueProvider.ProviderName, role.Name);

			List<Tuple<PermissionWithGrantedProviders, PermissionGrantInfoDto>> joinedPermissions = grantedPermissions
				.FullOuterJoin(role.Permissions,
					outer => outer.Name,
					inner => inner.Name,
					(roleName, outer, inner) => Tuple.Create(outer, inner))
				.ToList();

			foreach (Tuple<PermissionWithGrantedProviders, PermissionGrantInfoDto> tuple in joinedPermissions)
			{
				PermissionWithGrantedProviders permission = tuple.Item1;
				PermissionGrantInfoDto newPermission = tuple.Item2;
				if (newPermission == null)
				{
					if (permission.IsGranted)
					{
						await PermissionManager.SetAsync(permission.Name, RolePermissionValueProvider.ProviderName, role.Name, false);
					}
				}
				else if (permission == null)
				{
					throw new InvalidOperationException();
				}
				else
				{
					if (!permission.IsGranted)
					{
						await PermissionManager.SetAsync(permission.Name, RolePermissionValueProvider.ProviderName, role.Name, true);
					}
				}
			}

			return identityRole;
		}

		private async Task<RoleDto> ParseRole(FileInfo file)
		{
			XDocument doc;
			using (var reader = file.OpenText())
				doc = XDocument.Parse(await reader.ReadToEndAsync());

			Debug.Assert(doc.Root != null);
			if (doc.Root.Name.LocalName != "Role")
				throw new InvalidComObjectException();

			var result = new RoleDto
				{
					Permissions = new List<PermissionGrantInfoDto>(),
				};

			foreach (XAttribute attr in doc.Root.Attributes())
			{
				if (_setPropertyActionMap.TryGetValue(attr.Name.LocalName, out Action<RoleDto, XAttribute, IValueConverter> setAction))
					setAction(result, attr, Converter);
			}

			XElement permissionsElement = doc.Root.Element("Permissions");
			if (permissionsElement != null)
			{
				List<Tuple<PermissionDefinition, XElement>> joinedPermissions = PermissionDefinitionManager.GetPermissions()
					.Join(permissionsElement.Elements("Permission"),
						outer => outer.Name,
						inner => inner.Attribute(nameof(PermissionGrantInfoDto.Name))?.Value,
						Tuple.Create)
					.ToList();

				foreach (Tuple<PermissionDefinition, XElement> tuple in joinedPermissions)
				{
					PermissionDefinition permission = tuple.Item1;
					// XElement permissionElement = tuple.Item2;
					var permissionGrantInfo = new PermissionGrantInfoDto
						{
							Name = permission.Name,
							DisplayName = permission.DisplayName.Localize(StringLocalizerFactory),
							ParentName = permission.Parent?.Name,
							AllowedProviders = permission.Providers,
							GrantedProviders = new List<ProviderInfoDto>(),
							IsGranted = true,
						};
					permissionGrantInfo.GrantedProviders.Add(new ProviderInfoDto
						{
							ProviderName = RolePermissionValueProvider.ProviderName,
							ProviderKey = result.Name,
						});
					result.Permissions.Add(permissionGrantInfo);
				}
			}

			return result;
		}

		private DirectoryInfo GetAppDataDir(DataSeedContext context)
		{
			if (!context.Properties.TryGetValue(IsapDataSeederExtensions.AppDataDirPropertyName, out object value))
				throw new InvalidOperationException();

			switch (value)
			{
				case string appDataDirPath:
					return new DirectoryInfo(appDataDirPath);
				case DirectoryInfo appDataDir:
					return appDataDir;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
