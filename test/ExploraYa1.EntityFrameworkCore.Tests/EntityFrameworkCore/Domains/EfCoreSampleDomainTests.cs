using ExploraYa1.Samples;
using Xunit;

namespace ExploraYa1.EntityFrameworkCore.Domains;

[Collection(ExploraYa1TestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ExploraYa1EntityFrameworkCoreTestModule>
{

}
