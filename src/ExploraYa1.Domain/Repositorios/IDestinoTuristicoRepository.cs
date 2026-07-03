using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::ExploraYa1.Destinos;
using Volo.Abp.Domain.Repositories;


namespace ExploraYa1.Repositorios

{
    public interface IDestinoTuristicoRepository : IRepository<DestinoTuristico, Guid>
    {
        // Aquí podés agregar métodos específicos en el futuro
    }
}
