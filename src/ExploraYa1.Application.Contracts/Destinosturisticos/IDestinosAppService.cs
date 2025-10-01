using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Destinosturisticos
{
    public interface IDestinosAppService :
    
        ICrudAppService<
        DestinoTuristicoDTO,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearActualizarDestinoDTO>

    {
    }  
}
