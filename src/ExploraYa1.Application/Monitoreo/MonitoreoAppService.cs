using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploraYa1.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.Monitoreo;

[Authorize(ExploraYa1Permissions.Monitoreo.Default)]
public class MonitoreoAppService : ApplicationService, IMonitoreoAppService
{
    private readonly IRepository<ApiExternaLog, Guid> _logRepository;

    public MonitoreoAppService(IRepository<ApiExternaLog, Guid> logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<List<MetricasApiExternaDto>> GetMetricasAsync(
        DateTime? desde = null,
        DateTime? hasta = null)
    {
        var query = await _logRepository.GetQueryableAsync();

        if (desde.HasValue)
            query = query.Where(x => x.FechaHora >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(x => x.FechaHora <= hasta.Value);

        var result = query
            .GroupBy(x => x.NombreApi)
            .Select(g => new MetricasApiExternaDto
            {
                NombreApi        = g.Key,
                TotalLlamadas    = g.Count(),
                LlamadasExitosas = g.Count(x => x.Exitosa),
                LlamadasFallidas = g.Count(x => !x.Exitosa),
                TiempoPromedioMs = g.Average(x => x.TiempoMs),
                TiempoMaximoMs   = g.Max(x => x.TiempoMs),
                TiempoMinimoMs   = g.Min(x => x.TiempoMs),
                DesdeFecha       = desde,
                HastaFecha       = hasta
            })
            .ToList();

        return result;
    }
}
