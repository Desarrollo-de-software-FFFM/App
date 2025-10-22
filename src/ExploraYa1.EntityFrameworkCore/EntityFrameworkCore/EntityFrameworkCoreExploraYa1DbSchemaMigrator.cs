using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExploraYa1.Data;
using Volo.Abp.DependencyInjection;

namespace ExploraYa1.EntityFrameworkCore;

public class EntityFrameworkCoreExploraYa1DbSchemaMigrator
    : IExploraYa1DbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreExploraYa1DbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ExploraYa1DbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ExploraYa1DbContext>()
            .Database
            .MigrateAsync();
    }
}
