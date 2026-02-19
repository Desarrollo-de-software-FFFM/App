using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.Notificaciones;

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

        CreateMap<CalificacionDestino, CalificacionDto>()
            .ForMember(d => d.DestinoTuristicoId, opt => opt.MapFrom(s => s.DestinoTuristicoId));
        
        CreateMap<Notificacion, NotificacionDTO>();



    }
}
