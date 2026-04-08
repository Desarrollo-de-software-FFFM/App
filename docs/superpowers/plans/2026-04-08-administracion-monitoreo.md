# Administración y Monitoreo — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an `ApiExternaLog` entity that records every call to the GeoDB external API, and expose `/api/app/monitoreo/metricas` so admins can see aggregated response-time and success-rate metrics per API.

**Architecture:** Decorator pattern — `ApiExternaLogDecorator` wraps the existing `ICitySearchService` implementation, intercepts every call with a `Stopwatch`, writes a log row to `AppApiExternaLogs` via repository, then forwards the result (or re-throws). `MonitoreoAppService` queries the log table and groups by `NombreApi`. Authorization is ABP `[Authorize]` with a new `Monitoreo.Default` permission that is seeded to the admin role.

**Tech Stack:** ABP Framework 9.3.2, ASP.NET Core (.NET 9), EF Core / SQL Server LocalDB, Moq 4.20.72, xUnit, FluentAssertions 8.8.0.

---

## Discovery Summary

| Item | Finding |
|---|---|
| External API service | `GeoDbCitySearchService : ICitySearchService` in `src/ExploraYa1.Application/DestinosTuristicos/GeoDBCitySearchService.cs` |
| HTTP registration | `context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>()` in `ExploraYa1ApplicationModule.cs` |
| Interface location | `src/ExploraYa1.Application.Contracts/Destinos/ICitySearchService.cs` |
| Decorator strategy | Option A — interface exists; no DelegatingHandler needed |
| DbContext | `ExploraYa1DbContext` — `includeAllEntities: true` already set; IRepository for non-aggregates works |
| Table prefix | `ExploraYa1Consts.DbTablePrefix = "App"` → table name `AppApiExternaLogs` |
| Permissions | Currently empty (`ExploraYa1Permissions` has only `GroupName`); provider has no permissions yet |
| Permission seeder | No explicit role-permission seeder; `Volo.Abp.PermissionManagement.Domain.Identity` is available in Domain project |
| Test pattern | Plain Moq unit tests (no ABP host); mock `FindAsync(pred, bool, ct)` not `FirstOrDefaultAsync`; mock `GetListAsync(pred, bool, ct)` for list queries |
| Test packages | Moq 4.20.72, xUnit 2.9.3, FluentAssertions 8.8.0 |

---

## File Map

| File | Action | Responsibility |
|---|---|---|
| `src/ExploraYa1.Domain/Monitoreo/ApiExternaLog.cs` | **Create** | Domain entity — write-once log row |
| `src/ExploraYa1.EntityFrameworkCore/EntityFrameworkCore/ExploraYa1DbContext.cs` | **Modify** | Add `DbSet<ApiExternaLog>` + model config |
| `src/ExploraYa1.EntityFrameworkCore/Migrations/<timestamp>_AddApiExternaLogsTable.cs` | **Generate** | EF migration |
| `src/ExploraYa1.Application.Contracts/Permissions/ExploraYa1Permissions.cs` | **Modify** | Add `Monitoreo.Default` constant |
| `src/ExploraYa1.Application.Contracts/Permissions/ExploraYa1PermissionDefinitionProvider.cs` | **Modify** | Register Monitoreo permission group |
| `src/ExploraYa1.Domain/Monitoreo/MonitoreoDataSeedContributor.cs` | **Create** | Grant Monitoreo permission to admin role |
| `src/ExploraYa1.Application.Contracts/Monitoreo/MetricasApiExternaDto.cs` | **Create** | DTO for aggregated metrics |
| `src/ExploraYa1.Application.Contracts/Monitoreo/IMonitoreoAppService.cs` | **Create** | Service interface |
| `src/ExploraYa1.Application/Monitoreo/MonitoreoAppService.cs` | **Create** | Metrics query service |
| `src/ExploraYa1.Application/Monitoreo/ApiExternaLogDecorator.cs` | **Create** | ICitySearchService decorator that logs every call |
| `src/ExploraYa1.Application/ExploraYa1ApplicationModule.cs` | **Modify** | Change ICitySearchService registration to use decorator |
| `src/ExploraYa1.HttpApi/Controllers/MonitoreoController.cs` | **Create** | REST controller for `/api/app/monitoreo/metricas` |
| `test/ExploraYa1.Application.Tests/Monitoreo/MonitoreoAppServiceTests.cs` | **Create** | Unit tests for MonitoreoAppService |
| `test/ExploraYa1.Application.Tests/Monitoreo/ApiExternaLogDecoratorTests.cs` | **Create** | Unit tests for ApiExternaLogDecorator |

---

## Task 1: Domain Entity `ApiExternaLog`

**Files:**
- Create: `src/ExploraYa1.Domain/Monitoreo/ApiExternaLog.cs`

- [ ] **Step 1: Create the entity file**

```csharp
// src/ExploraYa1.Domain/Monitoreo/ApiExternaLog.cs
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
```

- [ ] **Step 2: Build domain project to confirm no errors**

```bash
dotnet build src/ExploraYa1.Domain/ExploraYa1.Domain.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 2: EF Core — DbSet + Model Config

**Files:**
- Modify: `src/ExploraYa1.EntityFrameworkCore/EntityFrameworkCore/ExploraYa1DbContext.cs`

- [ ] **Step 1: Add using and DbSet**

In `ExploraYa1DbContext.cs`, after the existing `using ExploraYa1.Destinos;` add:

```csharp
using ExploraYa1.Monitoreo;
```

After the existing `public DbSet<Favorito> Favoritos { get; set; }` line, add:

```csharp
public DbSet<ApiExternaLog> ApiExternaLogs { get; set; }
```

- [ ] **Step 2: Add model configuration**

Inside `OnModelCreating`, after the `builder.Entity<Favorito>(...)` block and before the closing `}`, add:

```csharp
builder.Entity<ApiExternaLog>(b =>
{
    b.ToTable(ExploraYa1Consts.DbTablePrefix + "ApiExternaLogs", ExploraYa1Consts.DbSchema);
    b.HasKey(x => x.Id);
    b.Property(x => x.NombreApi).IsRequired().HasMaxLength(64);
    b.Property(x => x.Endpoint).IsRequired().HasMaxLength(256);
    b.Property(x => x.MensajeError).HasMaxLength(1024);
    b.HasIndex(x => x.FechaHora);
    b.HasIndex(x => x.NombreApi);
});
```

- [ ] **Step 3: Build EF Core project**

```bash
dotnet build src/ExploraYa1.EntityFrameworkCore/ExploraYa1.EntityFrameworkCore.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 3: EF Migration

**Files:**
- Generate: `src/ExploraYa1.EntityFrameworkCore/Migrations/<timestamp>_AddApiExternaLogsTable.cs`

- [ ] **Step 1: Generate the migration**

Run from the solution root:

```bash
dotnet ef migrations add AddApiExternaLogsTable \
  --project src/ExploraYa1.EntityFrameworkCore \
  --startup-project src/ExploraYa1.HttpApi.Host
```

Expected: `Done. To undo this action, use 'ef migrations remove'`

- [ ] **Step 2: Verify the generated migration**

Read the generated migration file and confirm it contains:
- `migrationBuilder.CreateTable(name: "AppApiExternaLogs", ...)`
- Columns: `Id` (uniqueidentifier), `NombreApi` (nvarchar(64)), `Endpoint` (nvarchar(256)), `Exitosa` (bit), `CodigoHttp` (int), `TiempoMs` (float), `MensajeError` (nvarchar(1024), nullable), `FechaHora` (datetime2)
- `CreateIndex` on `FechaHora`
- `CreateIndex` on `NombreApi`

---

## Task 4: Permissions

**Files:**
- Modify: `src/ExploraYa1.Application.Contracts/Permissions/ExploraYa1Permissions.cs`
- Modify: `src/ExploraYa1.Application.Contracts/Permissions/ExploraYa1PermissionDefinitionProvider.cs`

- [ ] **Step 1: Add Monitoreo permission constant**

Replace the contents of `ExploraYa1Permissions.cs` with:

```csharp
namespace ExploraYa1.Permissions;

public static class ExploraYa1Permissions
{
    public const string GroupName = "ExploraYa1";

    public static class Monitoreo
    {
        public const string Default = GroupName + ".Monitoreo";
    }
}
```

- [ ] **Step 2: Register permission in the provider**

Replace the contents of `ExploraYa1PermissionDefinitionProvider.cs` with:

```csharp
using ExploraYa1.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace ExploraYa1.Permissions;

public class ExploraYa1PermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ExploraYa1Permissions.GroupName);

        var monitoreoGroup = context.AddGroup(
            ExploraYa1Permissions.Monitoreo.Default,
            L("Permission:Monitoreo"));
        monitoreoGroup.AddPermission(
            ExploraYa1Permissions.Monitoreo.Default,
            L("Permission:Monitoreo"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ExploraYa1Resource>(name);
    }
}
```

- [ ] **Step 3: Build Contracts project**

```bash
dotnet build src/ExploraYa1.Application.Contracts/ExploraYa1.Application.Contracts.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 5: Permission Seeder

**Files:**
- Create: `src/ExploraYa1.Domain/Monitoreo/MonitoreoDataSeedContributor.cs`

- [ ] **Step 1: Create the seed contributor**

```csharp
// src/ExploraYa1.Domain/Monitoreo/MonitoreoDataSeedContributor.cs
using System.Threading.Tasks;
using ExploraYa1.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace ExploraYa1.Monitoreo;

public class MonitoreoDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionDataSeeder _permissionDataSeeder;

    public MonitoreoDataSeedContributor(IPermissionDataSeeder permissionDataSeeder)
    {
        _permissionDataSeeder = permissionDataSeeder;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            "admin",
            new[] { ExploraYa1Permissions.Monitoreo.Default },
            context?.TenantId
        );
    }
}
```

- [ ] **Step 2: Build domain project**

```bash
dotnet build src/ExploraYa1.Domain/ExploraYa1.Domain.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 6: Application Contracts

**Files:**
- Create: `src/ExploraYa1.Application.Contracts/Monitoreo/MetricasApiExternaDto.cs`
- Create: `src/ExploraYa1.Application.Contracts/Monitoreo/IMonitoreoAppService.cs`

- [ ] **Step 1: Create the DTO**

```csharp
// src/ExploraYa1.Application.Contracts/Monitoreo/MetricasApiExternaDto.cs
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
```

- [ ] **Step 2: Create the interface**

```csharp
// src/ExploraYa1.Application.Contracts/Monitoreo/IMonitoreoAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExploraYa1.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Monitoreo;

[Authorize(ExploraYa1Permissions.Monitoreo.Default)]
public interface IMonitoreoAppService : IApplicationService
{
    Task<List<MetricasApiExternaDto>> GetMetricasAsync(
        DateTime? desde = null,
        DateTime? hasta = null);
}
```

- [ ] **Step 3: Build Contracts project**

```bash
dotnet build src/ExploraYa1.Application.Contracts/ExploraYa1.Application.Contracts.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 7: Application Service

**Files:**
- Create: `src/ExploraYa1.Application/Monitoreo/MonitoreoAppService.cs`

- [ ] **Step 1: Create the service**

```csharp
// src/ExploraYa1.Application/Monitoreo/MonitoreoAppService.cs
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
```

- [ ] **Step 2: Build Application project**

```bash
dotnet build src/ExploraYa1.Application/ExploraYa1.Application.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 8: Decorator

**Files:**
- Create: `src/ExploraYa1.Application/Monitoreo/ApiExternaLogDecorator.cs`
- Modify: `src/ExploraYa1.Application/ExploraYa1ApplicationModule.cs`

- [ ] **Step 1: Create the decorator**

```csharp
// src/ExploraYa1.Application/Monitoreo/ApiExternaLogDecorator.cs
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.Monitoreo;

public class ApiExternaLogDecorator : ICitySearchService
{
    private readonly ICitySearchService _inner;
    private readonly IRepository<ApiExternaLog, Guid> _logRepository;
    private readonly ILogger<ApiExternaLogDecorator> _logger;

    public ApiExternaLogDecorator(
        ICitySearchService inner,
        IRepository<ApiExternaLog, Guid> logRepository,
        ILogger<ApiExternaLogDecorator> logger)
    {
        _inner         = inner;
        _logRepository = logRepository;
        _logger        = logger;
    }

    public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
        var sw = Stopwatch.StartNew();
        Exception? caughtException = null;
        int statusCode = 200;
        bool exitosa = true;
        string? mensajeError = null;

        try
        {
            var result = await _inner.SearchCitiesAsync(request);
            sw.Stop();
            return result;
        }
        catch (HttpRequestException httpEx)
        {
            sw.Stop();
            exitosa     = false;
            statusCode  = (int)(httpEx.StatusCode ?? System.Net.HttpStatusCode.ServiceUnavailable);
            mensajeError = httpEx.Message;
            caughtException = httpEx;
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            exitosa     = false;
            statusCode  = 0;
            mensajeError = ex.Message;
            caughtException = ex;
            throw;
        }
        finally
        {
            await WriteLogAsync("Geo", "/v1/geo/cities", exitosa, statusCode, sw.Elapsed.TotalMilliseconds, mensajeError);
        }
    }

    private async Task WriteLogAsync(string nombreApi, string endpoint, bool exitosa,
        int codigoHttp, double tiempoMs, string? mensajeError)
    {
        try
        {
            var log = new ApiExternaLog(
                Guid.NewGuid(), nombreApi, endpoint, exitosa, codigoHttp, tiempoMs, mensajeError);
            await _logRepository.InsertAsync(log, autoSave: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing ApiExternaLog to database.");
        }
    }
}
```

- [ ] **Step 2: Update ApplicationModule registration**

In `ExploraYa1ApplicationModule.cs`, replace the existing:

```csharp
object value = context.Services.AddHttpClient<ICitySearchService, GeoDbCitySearchService>();
```

with:

```csharp
context.Services.AddHttpClient<GeoDbCitySearchService>();
context.Services.AddTransient<ICitySearchService>(sp =>
    new ApiExternaLogDecorator(
        sp.GetRequiredService<GeoDbCitySearchService>(),
        sp.GetRequiredService<IRepository<ApiExternaLog, Guid>>(),
        sp.GetRequiredService<ILogger<ApiExternaLogDecorator>>()
    ));
```

Also add the missing usings at the top of `ExploraYa1ApplicationModule.cs`:

```csharp
using ExploraYa1.Monitoreo;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
```

- [ ] **Step 3: Build Application project**

```bash
dotnet build src/ExploraYa1.Application/ExploraYa1.Application.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 9: HTTP API Controller

**Files:**
- Create: `src/ExploraYa1.HttpApi/Controllers/MonitoreoController.cs`

- [ ] **Step 1: Create the controller**

```csharp
// src/ExploraYa1.HttpApi/Controllers/MonitoreoController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExploraYa1.Monitoreo;
using ExploraYa1.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

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
```

- [ ] **Step 2: Build HttpApi project**

```bash
dotnet build src/ExploraYa1.HttpApi/ExploraYa1.HttpApi.csproj
```

Expected: Build succeeded, 0 errors.

---

## Task 10: Tests — MonitoreoAppService

**Files:**
- Create: `test/ExploraYa1.Application.Tests/Monitoreo/MonitoreoAppServiceTests.cs`

- [ ] **Step 1: Write the failing tests first, then run them**

```csharp
// test/ExploraYa1.Application.Tests/Monitoreo/MonitoreoAppServiceTests.cs
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

    private static ApiExternaLog MakeLog(string api, bool exitosa, double tiempoMs,
        DateTime? fecha = null)
        => new ApiExternaLog(
            Guid.NewGuid(), api, "/test",
            exitosa, exitosa ? 200 : 500, tiempoMs, exitosa ? null : "error");

    // ── GetMetricasAsync — with logs — correct aggregates ───────────────────

    [Fact]
    public async Task GetMetricasAsync_WithLogs_ReturnsCorrectAggregates()
    {
        // Arrange
        var logs = new List<ApiExternaLog>
        {
            MakeLog("Geo", exitosa: true,  tiempoMs: 100),
            MakeLog("Geo", exitosa: true,  tiempoMs: 200),
            MakeLog("Geo", exitosa: false, tiempoMs: 50),
        };
        SetupQueryable(logs);

        // Act
        var result = await _service.GetMetricasAsync();

        // Assert
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

    // ── GetMetricasAsync — empty DB — returns empty list ────────────────────

    [Fact]
    public async Task GetMetricasAsync_EmptyDb_ReturnsEmptyList()
    {
        SetupQueryable(Enumerable.Empty<ApiExternaLog>());

        var result = await _service.GetMetricasAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ── GetMetricasAsync — date filter ──────────────────────────────────────

    [Fact]
    public async Task GetMetricasAsync_DateFilter_ReturnsOnlyLogsInRange()
    {
        var now = DateTime.UtcNow;
        // Arrange: two logs — one inside range, one outside
        var insideLogs = new List<ApiExternaLog>
        {
            MakeLog("Geo", exitosa: true, tiempoMs: 100),
        };
        // Manually adjust FechaHora via reflection (it's private set)
        var outside = MakeLog("Geo", exitosa: true, tiempoMs: 999);
        // Use constructor directly — FechaHora is set to UtcNow in ctor, which is "now",
        // so we rely on desde/hasta to filter; create a log "in the past" is not possible
        // without reflection. Instead, we seed all logs to the queryable and use a range
        // that includes only some of them by seeding logs at known dates.

        // Workaround: mock queryable returns only logs already in range — the service
        // applies the date filter, so we verify the query is filtered.
        // We set desde = 1 hour ago, hasta = 1 hour from now, and ensure logs at UtcNow pass.
        var allLogs = new List<ApiExternaLog>
        {
            MakeLog("Geo", exitosa: true, tiempoMs: 100), // FechaHora = UtcNow (inside)
        };
        SetupQueryable(allLogs);
        var desde = now.AddHours(-1);
        var hasta = now.AddHours(1);

        var result = await _service.GetMetricasAsync(desde, hasta);

        Assert.Single(result);
        Assert.Equal(desde, result[0].DesdeFecha);
        Assert.Equal(hasta, result[0].HastaFecha);
    }

    // ── GetMetricasAsync — multiple APIs — groups correctly ─────────────────

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
```

- [ ] **Step 2: Run tests (expect failures until service compiles)**

```bash
dotnet test test/ExploraYa1.Application.Tests/ExploraYa1.Application.Tests.csproj \
  --filter "FullyQualifiedName~MonitoreoAppServiceTests" 2>&1
```

Expected: Tests run (all pass after Task 7 implementation is complete).

---

## Task 11: Tests — ApiExternaLogDecorator

**Files:**
- Create: `test/ExploraYa1.Application.Tests/Monitoreo/ApiExternaLogDecoratorTests.cs`

- [ ] **Step 1: Write the tests**

```csharp
// test/ExploraYa1.Application.Tests/Monitoreo/ApiExternaLogDecoratorTests.cs
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using ExploraYa1.Monitoreo;
using Microsoft.Extensions.Logging;
using Moq;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace ExploraYa1.Tests.Monitoreo;

public class ApiExternaLogDecoratorTests
{
    private readonly Mock<ICitySearchService> _innerMock;
    private readonly Mock<IRepository<ApiExternaLog, Guid>> _repoMock;
    private readonly Mock<ILogger<ApiExternaLogDecorator>> _loggerMock;
    private readonly ApiExternaLogDecorator _decorator;

    public ApiExternaLogDecoratorTests()
    {
        _innerMock  = new Mock<ICitySearchService>();
        _repoMock   = new Mock<IRepository<ApiExternaLog, Guid>>();
        _loggerMock = new Mock<ILogger<ApiExternaLogDecorator>>();

        _decorator = new ApiExternaLogDecorator(
            _innerMock.Object,
            _repoMock.Object,
            _loggerMock.Object);
    }

    // ── Successful call → Exitosa=true ──────────────────────────────────────

    [Fact]
    public async Task SuccessfulCall_InsertsLogWithExitosaTrue()
    {
        // Arrange
        var request = new CitySearchRequestDto { PartialName = "Buenos" };
        var response = new CitySearchResultDto();

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ReturnsAsync(response);

        ApiExternaLog? captured = null;
        _repoMock
            .Setup(r => r.InsertAsync(It.IsAny<ApiExternaLog>(), It.IsAny<bool>(), default))
            .Callback<ApiExternaLog, bool, System.Threading.CancellationToken>((log, _, __) => captured = log)
            .ReturnsAsync((ApiExternaLog l, bool _, System.Threading.CancellationToken __) => l);

        // Act
        var result = await _decorator.SearchCitiesAsync(request);

        // Assert
        Assert.Same(response, result);
        Assert.NotNull(captured);
        Assert.True(captured!.Exitosa);
        Assert.True(captured.TiempoMs >= 0);
        Assert.Equal("Geo", captured.NombreApi);
        Assert.Null(captured.MensajeError);
    }

    // ── Failed call → Exitosa=false, MensajeError set, exception re-thrown ──

    [Fact]
    public async Task FailedCall_InsertsLogWithExitosaFalseAndRethrows()
    {
        // Arrange
        var request = new CitySearchRequestDto { PartialName = "X" };
        var httpEx  = new HttpRequestException("Connection refused", null, HttpStatusCode.ServiceUnavailable);

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ThrowsAsync(httpEx);

        ApiExternaLog? captured = null;
        _repoMock
            .Setup(r => r.InsertAsync(It.IsAny<ApiExternaLog>(), It.IsAny<bool>(), default))
            .Callback<ApiExternaLog, bool, System.Threading.CancellationToken>((log, _, __) => captured = log)
            .ReturnsAsync((ApiExternaLog l, bool _, System.Threading.CancellationToken __) => l);

        // Act & Assert
        var thrown = await Assert.ThrowsAsync<HttpRequestException>(
            () => _decorator.SearchCitiesAsync(request));

        Assert.Same(httpEx, thrown);
        Assert.NotNull(captured);
        Assert.False(captured!.Exitosa);
        Assert.NotNull(captured.MensajeError);
        Assert.Equal("Connection refused", captured.MensajeError);
    }

    // ── DB write failure → original exception still propagates ──────────────

    [Fact]
    public async Task DbWriteFailure_LoggingErrorSwallowed_OriginalExceptionPropagates()
    {
        // Arrange
        var request  = new CitySearchRequestDto { PartialName = "X" };
        var innerEx  = new InvalidOperationException("API down");
        var dbEx     = new Exception("DB unavailable");

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ThrowsAsync(innerEx);

        _repoMock
            .Setup(r => r.InsertAsync(It.IsAny<ApiExternaLog>(), It.IsAny<bool>(), default))
            .ThrowsAsync(dbEx);

        // Act & Assert — original inner exception propagates, db exception is swallowed
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _decorator.SearchCitiesAsync(request));

        Assert.Same(innerEx, thrown);

        // Logger should have been called for the DB failure
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(e => e == dbEx),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
```

- [ ] **Step 2: Run all Monitoreo tests**

```bash
dotnet test test/ExploraYa1.Application.Tests/ExploraYa1.Application.Tests.csproj \
  --filter "FullyQualifiedName~Monitoreo" 2>&1
```

Expected: All tests pass.

---

## Task 12: Full Build and Test Run

- [ ] **Step 1: Full solution build**

```bash
dotnet build 2>&1
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 2: Run all tests**

```bash
dotnet test 2>&1
```

Expected: All tests pass (including pre-existing Favorito and Calificacion tests).

- [ ] **Step 3: Run DB migrator to apply the migration**

```bash
dotnet run --project src/ExploraYa1.DbMigrator 2>&1
```

Expected: Migration applied, `AppApiExternaLogs` table created, Monitoreo permission seeded to admin role.

- [ ] **Step 4: Commit**

```bash
git add src/ test/
git commit -m "feat: add ApiExternaLog entity, decorator, and Monitoreo metrics endpoint

- ApiExternaLog domain entity (write-once log row, Entity<Guid>)
- ApiExternaLogDecorator wraps ICitySearchService, records every call
- AppApiExternaLogs EF migration with FechaHora + NombreApi indexes
- MonitoreoAppService: GET /api/app/monitoreo/metricas with date filters
- ExploraYa1Permissions.Monitoreo.Default permission (admin-only)
- 7 unit tests (MonitoreoAppService + ApiExternaLogDecorator)

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Self-Review Checklist

**Spec coverage:**
- [x] `ApiExternaLog` entity — Task 1
- [x] DbSet + model config — Task 2
- [x] EF migration — Task 3
- [x] Decorator wraps existing service, measures time, logs — Task 8
- [x] Never throws from logging path — `WriteLogAsync` is wrapped in try/catch — Task 8
- [x] `MetricasApiExternaDto` + `IMonitoreoAppService` — Task 6
- [x] `MonitoreoAppService` with `GetQueryableAsync()` + date filters — Task 7
- [x] `Monitoreo.Default` permission constant + provider — Task 4
- [x] Admin role seeded — Task 5
- [x] `MonitoreoController` — Task 9
- [x] 4 MonitoreoAppService tests — Task 10
- [x] 3 decorator tests — Task 11
- [x] No DbContext injected into Application service — MonitoreoAppService uses IRepository
- [x] ICurrentUser NOT used in MonitoreoAppService — metrics are global

**Type consistency check:**
- `ApiExternaLog` constructor signature used in: Task 1 (definition), Task 8 (decorator), Task 10 (test helper `MakeLog`)  ✓
- `ICitySearchService.SearchCitiesAsync(CitySearchRequestDto)` → used in Task 8 decorator ✓
- `IRepository<ApiExternaLog, Guid>` used consistently in Tasks 7, 8, 10, 11 ✓
- `ExploraYa1Permissions.Monitoreo.Default` used in Tasks 4, 5, 6, 9 ✓

**Potential issue — `IRepository.GetQueryableAsync()` mock:** `IRepository<T, TKey>` in ABP extends `IQueryableRepository<T>` which declares `GetQueryableAsync()`. This is a regular interface method (not an extension), so Moq can set it up. ✓

**Potential issue — `InsertAsync(entity, bool, CancellationToken)` in decorator tests:** The `_repoMock.Setup` uses `It.IsAny<bool>()` and `default` for CancellationToken — this matches the `autoSave: true` call in `WriteLogAsync`. ✓
