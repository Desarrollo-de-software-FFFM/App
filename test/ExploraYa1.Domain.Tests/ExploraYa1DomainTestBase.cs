using Volo.Abp.Modularity;

namespace ExploraYa1;

/* Inherit from this class for your domain layer tests. */
public abstract class ExploraYa1DomainTestBase<TStartupModule> : ExploraYa1TestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
