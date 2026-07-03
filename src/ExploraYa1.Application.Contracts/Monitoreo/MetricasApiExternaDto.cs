using System;

namespace ExploraYa1.Monitoreo;

public class MetricasApiExternaDto
{
    public string NombreApi        { get; set; } = string.Empty;
    public int    TotalLlamadas    { get; set; }
    public int    LlamadasExitosas { get; set; }
    public int    LlamadasFallidas { get; set; }
    public double TiempoPromedioMs { get; set; }
    public double TiempoMaximoMs   { get; set; }
    public double TiempoMinimoMs   { get; set; }
    public DateTime? DesdeFecha    { get; set; }
    public DateTime? HastaFecha    { get; set; }
}
