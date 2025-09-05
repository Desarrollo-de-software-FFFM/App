using ExploraYa1.Samples;
using Xunit;

namespace ExploraYa1.EntityFrameworkCore.Applications;

[Collection(ExploraYa1TestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ExploraYa1EntityFrameworkCoreTestModule>
{

}
