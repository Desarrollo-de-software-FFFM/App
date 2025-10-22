using System.Threading.Tasks;

namespace ExploraYa1.Data;

public interface IExploraYa1DbSchemaMigrator
{
    Task MigrateAsync();
}
