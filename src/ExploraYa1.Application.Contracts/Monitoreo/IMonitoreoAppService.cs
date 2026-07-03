using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Monitoreo;

public interface IMonitoreoAppService : IApplicationService
{
    Task<List<MetricasApiExternaDto>> GetMetricasAsync(
        DateTime? desde = null,
        DateTime? hasta = null);
}
