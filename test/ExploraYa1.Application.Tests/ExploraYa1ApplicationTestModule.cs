using ExploraYa1.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ExploraYa1;

[DependsOn(
    typeof(ExploraYa1ApplicationModule),
    typeof(ExploraYa1DomainTestModule),
    typeof(ExploraYa1EntityFrameworkCoreModule)
)]
public class ExploraYa1ApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 1. Configurar la cadena de conexión ficticia (SQLite In-Memory)
        context.Services.Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = "DataSource=:memory:";
        });

        // 2. Reemplazar el proveedor de la base de datos (DB Provider)
        Configure<AbpDbContextOptions>(options =>
        {
            // Forzamos el uso de SQLite para los tests, reemplazando el SQL Server.
            //options.UseSqlite(); 
        });

        // 3. Opcional pero Recomendado: Desactivar la Unidad de Trabajo transaccional
        // Esto previene que los datos se reviertan automáticamente en cada test, 
        // lo que facilita la inspección de la base de datos en memoria.
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}