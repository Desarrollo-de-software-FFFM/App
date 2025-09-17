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
        public float latitud { get; set; }
        public float longuitud { get; set; }
        public string imagenUrl { get; set; }
        public int calificacionGeneral { get; set; }


    }
}
