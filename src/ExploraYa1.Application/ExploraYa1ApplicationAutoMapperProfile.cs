using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.Usuarios;
using Volo.Abp.Identity;

namespace ExploraYa1
{
    public class ExploraYa1ApplicationAutoMapperProfile : Profile
    {
        public ExploraYa1ApplicationAutoMapperProfile()
        {
            /* Configuración de AutoMapper */

            // Destinos turísticos
            CreateMap<DestinoTuristico, DestinoTuristicoDTO>();
            CreateMap<CrearActualizarDestinoDTO, DestinoTuristico>();

            // Calificaciones
            CreateMap<CalificacionDestino, CalificacionDto>();
            //.ForMember(d => d.DestinoTuristicoId, opt => opt.MapFrom(s => s.DestinoTuristicoId));
            // Lo comento porque si se deja el CreateMap<CalificacionDestino, CalificacionDto>() ya mapea automaticamente todas las propiedades que coinciden en nombre

            // Usuarios
            CreateMap<IdentityUser, UserProfileDto>();
        }
    }
}
