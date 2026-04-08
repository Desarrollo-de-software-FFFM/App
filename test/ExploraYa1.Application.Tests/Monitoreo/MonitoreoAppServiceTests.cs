using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploraYa1.Monitoreo;
using Moq;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace ExploraYa1.Tests.Monitoreo;

public class MonitoreoAppServiceTests
{
    private readonly Mock<IRepository<ApiExternaLog, Guid>> _repoMock;
    private readonly MonitoreoAppService _service;

    public MonitoreoAppServiceTests()
    {
        _repoMock = new Mock<IRepository<ApiExternaLog, Guid>>();
        _service  = new MonitoreoAppService(_repoMock.Object);
    }

    private void SetupQueryable(IEnumerable<ApiExternaLog> logs)
    {
        _repoMock
            .Setup(r => r.GetQueryableAsync())
            .ReturnsAsync(logs.AsQueryable());
    }

    private static ApiExternaLog MakeLog(string api, bool exitosa, double tiempoMs)
        => new ApiExternaLog(
            Guid.NewGuid(), api, "/test",
            exitosa, exitosa ? 200 : 500, tiempoMs, exitosa ? null : "error");

    [Fact]
    public async Task GetMetricasAsync_WithLogs_ReturnsCorrectAggregates()
    {
        var logs = new List<ApiExternaLog>
        {
            MakeLog("Geo", exitosa: true,  tiempoMs: 100),
            MakeLog("Geo", exitosa: true,  tiempoMs: 200),
            MakeLog("Geo", exitosa: false, tiempoMs: 50),
        };
        SetupQueryable(logs);

        var result = await _service.GetMetricasAsync();

        Assert.Single(result);
        var m = result[0];
        Assert.Equal("Geo", m.NombreApi);
        Assert.Equal(3, m.TotalLlamadas);
        Assert.Equal(2, m.LlamadasExitosas);
        Assert.Equal(1, m.LlamadasFallidas);
        Assert.Equal(116.67, m.TiempoPromedioMs, 2);
        Assert.Equal(200, m.TiempoMaximoMs);
        Assert.Equal(50,  m.TiempoMinimoMs);
    }

    [Fact]
    public async Task GetMetricasAsync_EmptyDb_ReturnsEmptyList()
    {
        SetupQueryable(Enumerable.Empty<ApiExternaLog>());

        var result = await _service.GetMetricasAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricasAsync_DateFilter_ReturnsOnlyLogsInRange()
    {
        var now = DateTime.UtcNow;
        var logs = new List<ApiExternaLog>
        {
            MakeLog("Geo", exitosa: true, tiempoMs: 100), // FechaHora = UtcNow (inside range)
        };
        SetupQueryable(logs);

        var desde = now.AddHours(-1);
        var hasta = now.AddHours(1);

        var result = await _service.GetMetricasAsync(desde, hasta);

        Assert.Single(result);
        Assert.Equal(desde, result[0].DesdeFecha);
        Assert.Equal(hasta, result[0].HastaFecha);
    }

    [Fact]
    public async Task GetMetricasAsync_MultipleLogs_GroupsByNombreApi()
    {
        var logs = new List<ApiExternaLog>
        {
            MakeLog("Geo",   exitosa: true,  tiempoMs: 100),
            MakeLog("Geo",   exitosa: false, tiempoMs: 200),
            MakeLog("Clima", exitosa: true,  tiempoMs: 50),
        };
        SetupQueryable(logs);

        var result = await _service.GetMetricasAsync();

        Assert.Equal(2, result.Count);

        var geo   = result.Single(r => r.NombreApi == "Geo");
        var clima = result.Single(r => r.NombreApi == "Clima");

        Assert.Equal(2, geo.TotalLlamadas);
        Assert.Equal(1, geo.LlamadasExitosas);
        Assert.Equal(1, geo.LlamadasFallidas);

        Assert.Equal(1, clima.TotalLlamadas);
        Assert.Equal(1, clima.LlamadasExitosas);
        Assert.Equal(0, clima.LlamadasFallidas);
    }
}
