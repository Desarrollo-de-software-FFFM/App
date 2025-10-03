using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ExploraYa1.Destinos
{
    public class Region: Entity<Guid>
    {
      

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }
        [Required]
        [StringLength(300)]
        public required string Descripcion { get; set; }

        public ICollection<DestinoTuristico> DestinosTuristicos { get; set; } = new List<DestinoTuristico>();

        public Pais Pais { get; set; }

        public Guid PaisId { get; set; }

    }
}
