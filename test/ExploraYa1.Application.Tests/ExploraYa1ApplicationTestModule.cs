using ExploraYa1.CalificacionesTest;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
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
        // Mantener FakeCurrentPrincipalAccessor como Singleton para los tests de autenticación
        context.Services.AddSingleton<ICurrentPrincipalAccessor, FakeCurrentPrincipalAccessor>();

        // ⚡ Asegurar que CrearCalificacionService sea Scoped y comparta DbContext
        context.Services.AddScoped<ICrearActualizarCalificacion, CrearCalificacionService>();

        // ⚡ Asegurar que CalificacionAppService sea Scoped (opcional, ABP por defecto los AppServices son Scoped)
        context.Services.AddScoped<CalificacionAppService>();
    }
}
