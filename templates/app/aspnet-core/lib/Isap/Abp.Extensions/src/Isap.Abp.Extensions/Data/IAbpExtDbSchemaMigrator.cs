using System.Threading.Tasks;

namespace Isap.Abp.Extensions.Data
{
	public interface IAbpExtDbSchemaMigrator
	{
		Task MigrateAsync();
	}
}
