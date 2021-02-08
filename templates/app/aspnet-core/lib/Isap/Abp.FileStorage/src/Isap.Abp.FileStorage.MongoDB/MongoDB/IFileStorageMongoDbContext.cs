using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Isap.Abp.FileStorage.MongoDB
{
    [ConnectionStringName(FileStorageDbProperties.ConnectionStringName)]
    public interface IFileStorageMongoDbContext : IAbpMongoDbContext
    {
        /* Define mongo collections here. Example:
         * IMongoCollection<Question> Questions { get; }
         */
    }
}
