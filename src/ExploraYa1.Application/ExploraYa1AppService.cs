using ExploraYa1.Localization;
using Volo.Abp.Application.Services;

namespace ExploraYa1;

/* Inherit your application services from this class.
 */
public abstract class ExploraYa1AppService : ApplicationService
{
    protected ExploraYa1AppService()
    {
        LocalizationResource = typeof(ExploraYa1Resource);
    }
}
