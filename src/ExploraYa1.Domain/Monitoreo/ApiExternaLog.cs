using System;
using Volo.Abp.Domain.Entities;

namespace ExploraYa1.Monitoreo;

public class ApiExternaLog : Entity<Guid>
{
    public string NombreApi     { get; private set; }
    public string Endpoint      { get; private set; }
    public bool   Exitosa       { get; private set; }
    public int    CodigoHttp    { get; private set; }
    public double TiempoMs      { get; private set; }
    public string? MensajeError { get; private set; }
    public DateTime FechaHora   { get; private set; }

    // Required no-arg ctor for EF
    private ApiExternaLog() { NombreApi = string.Empty; Endpoint = string.Empty; }

    public ApiExternaLog(Guid id, string nombreApi, string endpoint,
                         bool exitosa, int codigoHttp, double tiempoMs,
                         string? mensajeError)
    {
        Id           = id;
        NombreApi    = nombreApi;
        Endpoint     = endpoint;
        Exitosa      = exitosa;
        CodigoHttp   = codigoHttp;
        TiempoMs     = tiempoMs;
        MensajeError = mensajeError;
        FechaHora    = DateTime.UtcNow;
    }
}
