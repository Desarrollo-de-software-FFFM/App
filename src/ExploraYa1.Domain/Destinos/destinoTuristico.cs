using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.ExceptionHandling;
using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Destinos
{
    public class DestinoTuristico : AuditedAggregateRoot<Guid>
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [Range(0,int.MaxValue,ErrorMessage = "La poblacion debe ser un numero positivo")]
        public int Poblacion { get; set; }
        [Required]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public float Latitud { get; set; }
        [Required]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public float Longuitud { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "La URL de la imagen no puede superar los 200 caracteres.")]
        public string ImagenUrl { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "La calificación general debe estar entre 0 y 5")]
        public int CalificacionGeneral { get; set; }

        public Guid RegionId { get; set; }
        public Region Region { get; set; }

        //public Guid IdRegion { get; set; }
    }
}
