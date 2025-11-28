using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ExploraYa1.Destinos
{
    public class Pais: Entity<Guid>
    {
        
        [Required][StringLength(100)] public required string Nombre { get; set; }

        public ICollection<Region> Regiones { get; set; } = new List<Region>();
    } 

}
