using ExploraYa1.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ExploraYa1.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ExploraYa1Controller : AbpControllerBase
{
    protected ExploraYa1Controller()
    {
        LocalizationResource = typeof(ExploraYa1Resource);
    }
}
