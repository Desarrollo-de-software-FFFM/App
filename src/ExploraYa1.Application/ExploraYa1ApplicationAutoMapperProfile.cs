using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;

namespace ExploraYa1
{
    public class ExploraYa1ApplicationAutoMapperProfile : Profile
    {
        public ExploraYa1ApplicationAutoMapperProfile()
        {
            // Destinos
            CreateMap<DestinoTuristico, DestinoTuristicoDTO>();
            CreateMap<CrearActualizarDestinoDTO, DestinoTuristico>();

            // Calificaciones
            CreateMap<CalificacionDestino, CalificacionDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.DestinoTuristicoId, opt => opt.MapFrom(s => s.DestinoTuristicoId))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId))
                .ForMember(d => d.Puntuacion, opt => opt.MapFrom(s => s.Puntuacion))
                .ForMember(d => d.Comentario, opt => opt.MapFrom(s => s.Comentario))
                .ForMember(d => d.CreationTime, opt => opt.MapFrom(s => s.CreationTime));

            CreateMap<CrearActualizarCalificacionDTO, CalificacionDestino>();
        }
    }
}
