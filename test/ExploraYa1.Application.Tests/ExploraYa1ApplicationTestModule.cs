using ExploraYa1.CalificacionesTest;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace ExploraYa1;

[DependsOn(
    typeof(ExploraYa1ApplicationModule),
    typeof(ExploraYa1TestBaseModule)
)]
public class ExploraYa1ApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ICurrentPrincipalAccessor, FakeCurrentPrincipalAccessor>();
    }
}
