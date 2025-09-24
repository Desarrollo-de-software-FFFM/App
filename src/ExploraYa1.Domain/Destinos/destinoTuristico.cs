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
    public class destinoTuristico : AuditedAggregateRoot<Guid>
    {
        [Required]
        [StringLength(100)]
        public string nombre { get; set; }
        [Required]
        [Range(0,int.MaxValue,ErrorMessage = "La poblacion debe ser un numero positivo")]
        public int poblacion { get; set; }
        [Required]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public float latitud { get; set; }
        [Required]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public float longuitud { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "La URL de la imagen no puede superar los 200 caracteres.")]
        public string imagenUrl { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "La calificación general debe estar entre 0 y 5")]
        public int calificacionGeneral { get; set; }

        public Region Region { get; set; }
    }
}
