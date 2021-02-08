using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Isap.Abp.FileStorage.MongoDB
{
    public static class FileStorageMongoDbContextExtensions
    {
        public static void ConfigureFileStorage(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileStorageMongoModelBuilderConfigurationOptions(
                FileStorageDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);
        }
    }
}