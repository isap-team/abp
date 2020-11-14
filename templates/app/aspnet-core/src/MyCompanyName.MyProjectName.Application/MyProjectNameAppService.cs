using Isap.Abp.Extensions.Services;
using MyCompanyName.MyProjectName.Localization;

namespace MyCompanyName.MyProjectName
{
    /* Inherit your application services from this class.
     */
    public abstract class MyProjectNameAppService : AppServiceBase
    {
        protected MyProjectNameAppService()
        {
            LocalizationResource = typeof(MyProjectNameResource);
        }
    }
}
