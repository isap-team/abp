namespace Isap.Abp.FileStorage
{
	public static class FileStorageDbProperties
	{
		public const string ConnectionStringName = "Default";
		public static string DbTablePrefix { get; set; } = "Isap";

		public static string DbSchema { get; set; } = null;
	}
}
