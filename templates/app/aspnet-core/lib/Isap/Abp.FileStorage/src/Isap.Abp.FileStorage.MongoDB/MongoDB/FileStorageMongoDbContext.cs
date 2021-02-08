using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Isap.Abp.FileStorage.MongoDB
{
    [ConnectionStringName(FileStorageDbProperties.ConnectionStringName)]
    public class FileStorageMongoDbContext : AbpMongoDbContext, IFileStorageMongoDbContext
    {
        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureFileStorage();
        }
    }
}