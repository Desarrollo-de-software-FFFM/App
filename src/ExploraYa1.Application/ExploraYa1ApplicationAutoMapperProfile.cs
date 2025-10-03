using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;

namespace ExploraYa1;

public class ExploraYa1ApplicationAutoMapperProfile : Profile
{
    public ExploraYa1ApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<DestinoTuristico, DestinoTuristicoDTO>();
        CreateMap<CrearActualizarDestinoDTO, DestinoTuristico>();
         
        
        
        
       
    }
}
