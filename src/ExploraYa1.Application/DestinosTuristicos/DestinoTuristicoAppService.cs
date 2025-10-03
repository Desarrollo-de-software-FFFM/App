using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;

namespace ExploraYa1.DestinosTuristicos
{
    public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico, //The Book entity
        DestinoTuristicoDTO, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CrearActualizarDestinoDTO>, //Used to create/update a book
    IDestinoTuristicoAppService //implement the IBookAppService
    {
        public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository)
            : base(repository)
        {

        }
    }
}
