using Volo.Abp.Modularity;

namespace ExploraYa1;

[DependsOn(
    typeof(ExploraYa1ApplicationModule),
    typeof(ExploraYa1DomainTestModule)
)]
public class ExploraYa1ApplicationTestModule : AbpModule
{

}
