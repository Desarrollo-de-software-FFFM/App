using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExploraYa1.Monitoreo;
using ExploraYa1.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExploraYa1.Controllers;

[Area("app")]
[Route("api/app/monitoreo")]
[Authorize(ExploraYa1Permissions.Monitoreo.Default)]
public class MonitoreoController : ExploraYa1Controller
{
    private readonly IMonitoreoAppService _monitoreoAppService;

    public MonitoreoController(IMonitoreoAppService monitoreoAppService)
        => _monitoreoAppService = monitoreoAppService;

    [HttpGet("metricas")]
    public Task<List<MetricasApiExternaDto>> GetMetricasAsync(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta)
        => _monitoreoAppService.GetMetricasAsync(desde, hasta);
}
