using Volo.Abp.Reflection;

namespace Isap.Abp.FileStorage.Permissions
{
	public class FileStoragePermissions
	{
		public const string GroupName = "FileStorage";

		public static string[] GetAll()
		{
			return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileStoragePermissions));
		}

		public static class FileDataPermissions
		{
			public const string Default = GroupName + ".FileData";
			public const string Create = Default + ".Create";
			public const string Update = Default + ".Update";
			public const string UpdateOwn = Update + ".Own";
			public const string UpdateOther = Update + ".Other";
			public const string Delete = Default + ".Delete";
			public const string DeleteOwn = Delete + ".Own";
			public const string DeleteOther = Delete + ".Other";
			public const string ManagePermissions = Default + ".ManagePermissions";
		}
	}
}
