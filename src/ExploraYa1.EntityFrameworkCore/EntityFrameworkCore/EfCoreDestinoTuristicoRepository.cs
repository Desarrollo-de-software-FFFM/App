using ExploraYa1.Destinos;
using ExploraYa1.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ExploraYa1.EntityFrameworkCore
{
    public class EfCoreDestinoTuristicoRepository
        : EfCoreRepository<ExploraYa1DbContext, DestinoTuristico, Guid>,
          IDestinoTuristicoRepository
    {
        public EfCoreDestinoTuristicoRepository(IDbContextProvider<ExploraYa1DbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
