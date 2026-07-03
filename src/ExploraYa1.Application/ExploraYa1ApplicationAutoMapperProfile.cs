using AutoMapper;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.Experiencias; // <--- Asegúrate de tener este using
using ExploraYa1.Notificaciones;
using ExploraYa1.Usuarios;
using Volo.Abp.Identity;

namespace ExploraYa1
{
    public class ExploraYa1ApplicationAutoMapperProfile : Profile
    {
        public ExploraYa1ApplicationAutoMapperProfile()
        {
            /* Configuración de AutoMapper */
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

  
        CreateMap<DestinoTuristico, DestinoTuristicoDTO>();
        CreateMap<CrearActualizarDestinoDTO, DestinoTuristico>();

            // Destinos turísticos
            CreateMap<DestinoTuristico, DestinoTuristicoDTO>();
            CreateMap<CrearActualizarDestinoDTO, DestinoTuristico>();

            // Calificaciones
            CreateMap<CalificacionDestino, CalificacionDto>();
            //.ForMember(d => d.DestinoTuristicoId, opt => opt.MapFrom(s => s.DestinoTuristicoId));
            // Lo comento porque si se deja el CreateMap<CalificacionDestino, CalificacionDto>() ya mapea automaticamente todas las propiedades que coinciden en nombre

            // Usuarios
            CreateMap<IdentityUser, UserProfileDto>();
        

        CreateMap<Notificacion, NotificacionDTO>();


        CreateMap<Experiencia, ExperienciaDto>();

        CreateMap<CrearActualizarExperienciaDto, Experiencia>();
        }
}
}
