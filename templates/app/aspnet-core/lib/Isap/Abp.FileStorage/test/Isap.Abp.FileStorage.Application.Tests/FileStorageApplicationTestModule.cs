using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage
{
    [DependsOn(
        typeof(FileStorageApplicationModule),
        typeof(FileStorageDomainTestModule)
        )]
    public class FileStorageApplicationTestModule : AbpModule
    {

    }
}
