using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExploraYa1.Destinos
{
    public class Pais
    {
        [Required] [StringLength(100)] public required string Nombre { get; set; }
    }
}
