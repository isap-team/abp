using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Isap.Abp.FileStorage
{
    [DependsOn(
        typeof(FileStorageHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class FileStorageConsoleApiClientModule : AbpModule
    {
        
    }
}
