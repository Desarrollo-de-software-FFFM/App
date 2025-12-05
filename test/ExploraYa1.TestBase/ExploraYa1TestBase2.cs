using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using ExploraYa1.EntityFrameworkCore;

namespace ExploraYa1.TestBase
{
    [DependsOn(
        typeof(ExploraYa1ApplicationModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(ExploraYa1TestBaseModule)
    )]
    public class ExploraYa1TestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 🔹 Registrar InMemory DbContext
            context.Services.AddDbContext<ExploraYa1DbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // 🔹 Registrar repositorios ABP
            context.Services.AddAbpDbContext<ExploraYa1DbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });
        }
    }
}
