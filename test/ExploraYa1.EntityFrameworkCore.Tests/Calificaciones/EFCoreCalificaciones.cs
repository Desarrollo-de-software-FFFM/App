using ExploraYa1.Destinos;
using ExploraYa1.EntityFrameworkCore;
using ExploraYa1.CalificacionesTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace ExploraYa1.Calificaciones
{
    [Collection(ExploraYa1TestConsts.CollectionDefinitionName)]
    public class EFCoreOpinionesAppService_Test : OpinionTestAppServiceTest<ExploraYa1EntityFrameworkCoreTestModule>
    {
    }
}
