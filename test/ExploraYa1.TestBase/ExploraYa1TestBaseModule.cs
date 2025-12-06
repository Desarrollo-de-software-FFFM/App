using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using System;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Users;

namespace ExploraYa1;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
)]
public class ExploraYa1TestBaseModule : AbpModule
{
    // ⭐ NECESARIO → Tu clase base la usa
    public static ICurrentUser CurrentUserMock;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();

        // ⭐ Crear mock de ICurrentUser
        var userMock = Substitute.For<ICurrentUser>();
        userMock.IsAuthenticated.Returns(true);
        userMock.Id.Returns(Guid.NewGuid());

        // ⭐ Guardamos el mock para que la clase base pueda modificarlo
        CurrentUserMock = userMock;

        // ⭐ Reemplazamos el servicio en el contenedor
        context.Services.Replace(ServiceDescriptor.Singleton<ICurrentUser>(userMock));
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
