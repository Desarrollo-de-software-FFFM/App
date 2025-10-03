using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.Destinosturisticos;

namespace ExploraYa1;

public class ExploraYa1ApplicationAutoMapperProfile : Profile
{
    public ExploraYa1ApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<DestinoTuristico, Destinosturisticos.DestinoTuristicoDTO>();
        CreateMap<Destinosturisticos.CrearActualizarDestinoDTO, DestinoTuristico>();
         
        
        
        
       
    }
}
