using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExploraYa1.Destinos
{
    public class Region
    {
        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }
        [Required]
        [StringLength(300)]
        public required string Descripcion { get; set; }

        public ICollection<destinoTuristico> DestinosTuristicos { get; set; }

        public Pais Pais { get; set; }

    }
}
