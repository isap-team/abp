namespace Isap.Abp.Extensions.Clustering
{
	public interface IClusterNode
	{
		int Id { get; }
		string ApplicationRole { get; }
	}

	public class ClusterNode: IClusterNode
	{
		public ClusterNode()
		{
		}

		public ClusterNode(int id, string applicationRole)
		{
			Id = id;
			ApplicationRole = applicationRole;
		}

		public int Id { get; set; }
		public string ApplicationRole { get; set; }
	}
}
