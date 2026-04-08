using System;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using System.Net.Http;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.Monitoreo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

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
        context.Services.AddHttpClient<GeoDbCitySearchService>();
        context.Services.AddTransient<ICitySearchService>(sp =>
            new ApiExternaLogDecorator(
                sp.GetRequiredService<GeoDbCitySearchService>(),
                sp.GetRequiredService<IRepository<ApiExternaLog, Guid>>(),
                sp.GetRequiredService<ILogger<ApiExternaLogDecorator>>()
            ));
        context.Services.AddTransient<ICrearActualizarCalificacion, CrearCalificacionService>();
    }
}
