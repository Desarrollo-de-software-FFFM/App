using ExploraYa1.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace ExploraYa1.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ExploraYa1EntityFrameworkCoreModule),
    typeof(ExploraYa1ApplicationContractsModule)
)]
public class ExploraYa1DbMigratorModule : AbpModule
{
}
