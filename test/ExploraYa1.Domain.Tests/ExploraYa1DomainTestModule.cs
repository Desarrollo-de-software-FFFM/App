using Volo.Abp.Modularity;

namespace ExploraYa1;

[DependsOn(
    typeof(ExploraYa1DomainModule),
    typeof(ExploraYa1TestBaseModule)
)]
public class ExploraYa1DomainTestModule : AbpModule
{

}
