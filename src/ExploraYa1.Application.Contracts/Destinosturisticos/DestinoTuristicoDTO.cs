using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;


namespace ExploraYa1.Destinosturisticos
{
    public class DestinoTuristicoDTO: AuditedEntityDto<Guid>

    {
        public string Nombre { get; set; }
        public int Poblacion { get; set; }
        public float Latitud { get; set; }
        public float Longuitud { get; set; }
        public string ImagenUrl { get; set; }
        public int CalificacionGeneral { get; set; }
        public Guid RegionId { get; set; }



        
    }
        public class RegionDTO: EntityDto<Guid>
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Guid PaisId { get; set; }
    }
        public class PaisDTO: EntityDto<Guid>
    {
        public string Nombre { get; set; }
   }   
}
