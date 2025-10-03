using System;
using Xunit;
using Volo.Abp.Testing;
using System.Collections.Generic;
using ExploraYa1.EntityFrameworkCore;


namespace ExploraYa1.Destinos
{
    [Collection(ExploraYa1TestConsts.CollectionDefinitionName)]


    public class EFCoreDestinoTuristicoAppService_tests : DestinoTuristicoAppService_tests<ExploraYa1EntityFrameworkCoreTestModule>
    {
    }
}
