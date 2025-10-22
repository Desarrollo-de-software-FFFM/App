using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ExploraYa1.Data;

/* This is used if database provider does't define
 * IExploraYa1DbSchemaMigrator implementation.
 */
public class NullExploraYa1DbSchemaMigrator : IExploraYa1DbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
