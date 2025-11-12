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
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();
        
        var currentUserMock = Substitute.For<ICurrentUser>();
        currentUserMock.IsAuthenticated.Returns(true);
        currentUserMock.Id.Returns(Guid.NewGuid());
        context.Services.Replace(ServiceDescriptor.Singleton<ICurrentUser>(currentUserMock));

        context.Services.AddAlwaysAllowAuthorization();




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
