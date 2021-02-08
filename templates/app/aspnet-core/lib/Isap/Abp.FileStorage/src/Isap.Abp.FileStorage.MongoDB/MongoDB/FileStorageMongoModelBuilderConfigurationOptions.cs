using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace Isap.Abp.FileStorage.MongoDB
{
    public class FileStorageMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public FileStorageMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}