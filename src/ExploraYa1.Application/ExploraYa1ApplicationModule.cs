using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.OpenIddict;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.OpenIddict;
using ExploraYa1.EntityFrameworkCore;


namespace ExploraYa1;

[DependsOn(
    typeof(ExploraYa1DomainModule),
    typeof(ExploraYa1ApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(ExploraYa1EntityFrameworkCoreModule)

)]
public class ExploraYa1ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ExploraYa1ApplicationModule>();
        });

        // GeoDB API
        context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>();

        // Calificaciones
        context.Services.AddTransient<ICrearActualizarCalificacion, CrearCalificacionService>();

        // 🔥 REGISTRO CORRECTO DEL DATA SEED CONTRIBUTOR
        context.Services.AddTransient<IDataSeedContributor, OpenIddictDataSeedContributor>();
        context.Services.AddTransient<OpenIddictDataSeedContributor>();

    }
}

