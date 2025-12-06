using Volo.Abp.Modularity;
using Volo.Abp.Users;

namespace ExploraYa1;

public abstract class ExploraYa1ApplicationTestBase<TStartupModule> : ExploraYa1TestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected ICurrentUser CurrentUser => GetRequiredService<ICurrentUser>();

    protected ICurrentUser CurrentUserMock => ExploraYa1TestBaseModule.CurrentUserMock;
}
