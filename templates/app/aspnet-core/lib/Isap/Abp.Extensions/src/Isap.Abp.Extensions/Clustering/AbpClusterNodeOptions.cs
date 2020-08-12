namespace Isap.Abp.Extensions.Clustering
{
	public class AbpClusterNodeOptions
	{
		public const string DefaultApplicationRole = "Default";

		public bool UseSharding { get; set; } = false;
		public int NodeId { get; set; } = 0;
		public string ApplicationRole { get; set; } = DefaultApplicationRole;
	}
}
