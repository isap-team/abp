using Isap.Abp.FileStorage.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Isap.Abp.FileStorage.Permissions
{
	public class FileStoragePermissionDefinitionProvider: PermissionDefinitionProvider
	{
		public override void Define(IPermissionDefinitionContext context)
		{
			PermissionGroupDefinition group = context.AddGroup(FileStoragePermissions.GroupName, L("Permission:FileStorage"));

			var fileDataPermissions = group.AddPermission(FileStoragePermissions.FileDataPermissions.Default, L("Permission:FileDataPermissionManagement"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.Create, L("Permission:Create"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.Update, L("Permission:Edit"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.UpdateOwn, L("Permission:EditOwn"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.UpdateOther, L("Permission:EditOther"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.Delete, L("Permission:Delete"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.DeleteOwn, L("Permission:DeleteOwn"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.DeleteOther, L("Permission:DeleteOther"));
			fileDataPermissions.AddChild(FileStoragePermissions.FileDataPermissions.ManagePermissions, L("Permission:ManagePermissions"));
		}

		private static LocalizableString L(string name)
		{
			return LocalizableString.Create<FileStorageResource>(name);
		}
	}
}
