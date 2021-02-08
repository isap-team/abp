using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Clustering
{
	public interface ICurrentNode
	{
		bool UseSharding { get; }
		int Id { get; }
		string ApplicationRole { get; }
	}

	public class CurrentNode: ICurrentNode, ISingletonDependency
	{
		private readonly AbpClusterNodeOptions _clusterNodeOption;

		public CurrentNode(
			IOptions<AbpClusterNodeOptions> options)
		{
			_clusterNodeOption = options.Value;
		}

		public bool UseSharding => _clusterNodeOption.UseSharding;
		public int Id => _clusterNodeOption.NodeId;
		public string ApplicationRole => _clusterNodeOption.ApplicationRole;
	}
}
