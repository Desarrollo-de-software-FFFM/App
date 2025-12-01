using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.OpenIddict;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;


namespace ExploraYa1;
[DependsOn(
    typeof(ExploraYa1DomainModule),
    typeof(ExploraYa1ApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    
)]

public class ExploraYa1ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ExploraYa1ApplicationModule>();
        });

        // Registra el servicio HttpClient para GeoDbCitySearchService
         object value = context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>();
           context.Services.AddTransient<ICrearActualizarCalificacion, CrearCalificacionService>();
        context.Services.AddTransient<OpenIddictDataSeedContributor>();

    }
    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var dataSeeder = context.ServiceProvider.GetRequiredService<IDataSeeder>();
        await dataSeeder.SeedAsync();
    }



}
