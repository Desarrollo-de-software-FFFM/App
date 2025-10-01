using AutoMapper;

namespace ExploraYa1;

public class ExploraYa1ApplicationAutoMapperProfile : Profile
{
    public ExploraYa1ApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Destinosturisticos.DestinoTuristicoDTO, Destinosturisticos.DestinoTuristicoDTO>();
        CreateMap<Destinosturisticos.CrearActualizarDestinoDTO, Destinosturisticos.DestinoTuristicoDTO>();
        CreateMap<Destinosturisticos.DestinoTuristicoDTO, Destinosturisticos.RegionDTO>();
        CreateMap<Destinosturisticos.CrearActualizarRegionDTO, Destinosturisticos.RegionDTO>();
        CreateMap<Destinosturisticos.DestinoTuristicoDTO, Destinosturisticos.PaisDTO>();
        CreateMap<Destinosturisticos.CrearActualizarPaisDTO, Destinosturisticos.PaisDTO>();
    }
}
