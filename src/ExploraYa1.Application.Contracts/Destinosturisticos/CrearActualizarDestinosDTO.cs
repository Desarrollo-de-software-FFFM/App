using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Destinosturisticos
{
    public class CrearActualizarDestinoDTO
    {
    
            public string Nombre { get; set; }
            public int Poblacion { get; set; }
            public float Latitud { get; set; }
            public float Longuitud { get; set; }
            public string ImagenUrl { get; set; }
            public int CalificacionGeneral { get; set; }
            public Guid IdRegion { get; set; }
     
     
    }
    public class CrearActualizarRegionDTO
    {
            public string Nombre { get; set; }
             public string Descripcion { get; set; }
            public Guid PaisId { get; set; }

    }
             public class CrearActualizarPaisDTO
   {
             public string Nombre { get; set; }
      

   }
}
