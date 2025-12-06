using ExploraYa1.CalificacionesTest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Testing;
using Volo.Abp.Uow;
using Volo.Abp.Users;


namespace ExploraYa1;

public abstract class ExploraYa1TestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected override void BeforeAddApplication(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", false);
        builder.AddJsonFile("appsettings.secrets.json", true);
        services.ReplaceConfiguration(builder.Build());
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                await action();

                await uow.CompleteAsync();
            }
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                var result = await func();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
    protected void LoginAs(Guid userId)
    {
        var accessor = ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>()
                        as FakeCurrentPrincipalAccessor;

        if (accessor == null)
            throw new Exception("FakeCurrentPrincipalAccessor no está siendo usado por el container de ABP.");

        accessor.SetPrincipal(new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, "testuser"),
            new Claim(AbpClaimTypes.Email, "test@example.com")
            }, "TestAuth")
        ));
    }


    protected void Logout()
    {
        var accessor = ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>()
                        as FakeCurrentPrincipalAccessor;

        accessor?.SetPrincipal(new ClaimsPrincipal(new ClaimsIdentity()));
    }
    protected void ReplaceService<TService>(TService instance)
    {
        var services = GetRequiredService<IServiceCollection>();
        services.Replace(ServiceDescriptor.Singleton(typeof(TService), instance));
    }
    protected void ForceSetCurrentUser(ICurrentUser newUser)
    {
        var field = typeof(ServiceProvider).GetField("_engine",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var engine = field.GetValue(ServiceProvider);

        var callSiteFactoryField = engine.GetType().GetField("_callSiteFactory",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var callSiteFactory = callSiteFactoryField.GetValue(engine);

        var descriptorLookupField = callSiteFactory.GetType().GetField("_descriptorLookup",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var lookup = (Dictionary<Type, ServiceDescriptor[]>)descriptorLookupField.GetValue(callSiteFactory);

        lookup[typeof(ICurrentUser)] = new[]
        {
        new ServiceDescriptor(typeof(ICurrentUser), newUser)
    };
    }






}

