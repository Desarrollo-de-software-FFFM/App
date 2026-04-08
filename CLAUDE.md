# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ExploraYa1 is a full-stack tourist destination management app:
- **Backend**: ASP.NET Core (.NET 9) with ABP Framework v9.3.2
- **Frontend**: Angular 20 (SPA)
- **Database**: SQL Server LocalDB via Entity Framework Core
- **Auth**: OpenIddict (OAuth 2.0/OpenID Connect)

## Commands

### Backend (.NET)
```bash
dotnet build
dotnet run --project src/ExploraYa1.HttpApi.Host          # API at https://localhost:44391
dotnet run --project src/ExploraYa1.DbMigrator            # Apply DB migrations
dotnet test                                                 # All tests
dotnet test --filter "FullyQualifiedName~ClassName"        # Single test class
```

### EF Migrations
```bash
# Add a migration (run from solution root)
dotnet ef migrations add <MigrationName> \
  --project src/ExploraYa1.EntityFrameworkCore \
  --startup-project src/ExploraYa1.HttpApi.Host

# dotnet-ef must be installed globally first:
dotnet tool install --global dotnet-ef
```

### Frontend (Angular — run from `angular/` directory)
```bash
yarn install          # or npm install
npx ng serve          # Dev server at http://localhost:4200
npx ng build
npx ng test --watch=false --browsers=ChromeHeadless   # Karma unit tests (headless)
npx ng lint
```

### Initial Setup
```bash
abp install-libs      # Install ABP client-side dependencies (run at solution root)
```

## Architecture

The solution follows ABP Framework's layered DDD architecture:

| Project | Role |
|---|---|
| `ExploraYa1.Domain` | Aggregate roots, domain entities, business rules |
| `ExploraYa1.Domain.Shared` | Shared constants, enums, error codes |
| `ExploraYa1.Application.Contracts` | DTOs, service interfaces (decouples API from logic) |
| `ExploraYa1.Application` | Application services implementing contracts |
| `ExploraYa1.EntityFrameworkCore` | DbContext, EF Core config, migrations, repositories |
| `ExploraYa1.HttpApi` | REST controllers |
| `ExploraYa1.HttpApi.Host` | Entry point — DI, middleware, auth, Swagger |
| `ExploraYa1.DbMigrator` | Console app for applying EF migrations |

**Key domain entities:**
- `DestinoTuristico` — tourist destination (inherits `AuditedAggregateRoot<Guid>`)
- `Favorito` — user-saved destinations (links UserId ↔ DestinoTuristicoId); unique index on `(UserId, DestinoTuristicoId)`
- `CalificacionDestino` — user ratings for destinations
- `Region` / `Pais` — geographic hierarchy (country → region → destination)
- `ApiExternaLog` — write-once log of every external HTTP call (inherits `Entity<Guid>`; no audit columns)

**ABP patterns in use:**
- `CrudAppService<TEntity, TDto, TKey>` as base for standard CRUD services
- `IRepository<T, TKey>` for all data access — inject repositories, don't use DbContext directly in services
- AutoMapper for DTO ↔ entity mapping (mappings configured in `*ApplicationAutoMapperProfile`)
- ABP modules: each layer has a `*Module.cs` that registers dependencies and imports other modules
- `ICurrentUser.GetId()` to resolve the logged-in user — never accept a raw `userId` from HTTP callers
- `[Authorize]` on service classes to require authentication at the ABP layer
- **Do NOT write explicit controllers** for `IApplicationService` implementations — ABP's dynamic C# API auto-generates REST endpoints (e.g. `MonitoreoAppService.GetMetricasAsync` → `GET /api/app/monitoreo/metricas`). A hand-written controller at the same route causes a Swagger 500 (`SwaggerGeneratorException: Conflicting method/path combination`)

## Testing

Tests live in `test/`. The test base project (`ExploraYa1.TestBase`) provides shared fixtures.

- Domain tests: pure unit tests, no DB
- Application tests: two styles in use:
  - ABP integration style (`ExploraYa1ApplicationTestBase<TStartupModule>`) — uses real ABP DI, NSubstitute for `ICurrentUser`
  - Plain unit style (Moq, no ABP host) — used for `FavoritoAppService`
- EF Core tests: integration tests against a real (LocalDB) database

### Moq + ABP repositories — critical rule
`FirstOrDefaultAsync(predicate)` is an **extension method** on `IReadOnlyRepository<T>` and **cannot be mocked with Moq**.
Always mock `FindAsync(predicate, includeDetails, cancellationToken)` — the real interface method that the extension calls internally:

```csharp
// WRONG — throws NotSupportedException at setup time
repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<...>(), It.IsAny<CancellationToken>()));

// CORRECT
repoMock.Setup(r => r.FindAsync(
    It.IsAny<Expression<Func<T, bool>>>(),
    It.IsAny<bool>(),              // includeDetails
    It.IsAny<CancellationToken>()))
    .ReturnsAsync(entity);
```

Similarly, `GetListAsync(predicate, includeDetails, cancellationToken)` IS a direct interface method and is mockable.

`GetQueryableAsync()` is also a direct interface method on `IQueryableRepository<T>` and IS mockable:

```csharp
repoMock.Setup(r => r.GetQueryableAsync())
        .ReturnsAsync(list.AsQueryable());
```

Use this pattern when the service uses LINQ GroupBy / aggregation (e.g., `MonitoreoAppService`).

### Guid IDs in domain entities
ABP EF Core repositories auto-generate Guid IDs on `InsertAsync`. Plain Moq mocks do **not**.
To make entities testable without the real repository, assign `Id = Guid.NewGuid()` in the domain entity constructor.

### Angular tests
Run with `npx ng test --watch=false --browsers=ChromeHeadless` from the `angular/` directory.
Use `jasmine.SpyObj` and provide the spy via `TestBed.configureTestingModule providers`.

## Key Configuration

`src/ExploraYa1.HttpApi.Host/appsettings.json`:
- API self URL: `https://localhost:44391`
- Angular URL / CORS: `http://localhost:4200`
- DB connection: `Server=(LocalDb)\MSSQLLocalDB;Database=ExploraYa1`

## Favoritos feature (branch feature/23-favoritosfranco)

| Layer | File |
|---|---|
| Domain entity | `src/ExploraYa1.Domain/Destinos/Favoritos.cs` |
| DTO | `src/ExploraYa1.Application.Contracts/Destinos/FavoritoDto.cs` |
| Interface | `src/ExploraYa1.Application.Contracts/Destinos/IFavoritoAppService.cs` |
| Service | `src/ExploraYa1.Application/DestinosTuristicos/FavoritoAppService.cs` |
| EF config | `src/ExploraYa1.EntityFrameworkCore/EntityFrameworkCore/ExploraYa1DbContext.cs` |
| Migration | `src/ExploraYa1.EntityFrameworkCore/Migrations/20260331165524_AddFavoritosTable.cs` |
| Angular service | `angular/src/app/favoritos/favorito.service.ts` |
| Angular component | `angular/src/app/favoritos/favoritos.component.ts` |
| Angular tests | `angular/src/app/favoritos/favoritos.component.spec.ts` |

**API methods:**
- `POST /api/app/favorito/agregar-favorito?destinoTuristicoId={id}` — add favorite
- `DELETE /api/app/favorito/eliminar-favorito?destinoTuristicoId={id}` — remove (throws `EntityNotFoundException` if not found)
- `GET /api/app/favorito/obtener-mis-favoritos` — list current user's favorites

**After cloning / DB reset:** run `dotnet run --project src/ExploraYa1.DbMigrator` to apply the `AppFavoritos` table migration.

## Monitoreo feature (branch feature/28-admymonitoreofranco)

| Layer | File |
|---|---|
| Domain entity | `src/ExploraYa1.Domain/Monitoreo/ApiExternaLog.cs` |
| Permission seeder | `src/ExploraYa1.Domain/Monitoreo/MonitoreoDataSeedContributor.cs` |
| DTO | `src/ExploraYa1.Application.Contracts/Monitoreo/MetricasApiExternaDto.cs` |
| Interface | `src/ExploraYa1.Application.Contracts/Monitoreo/IMonitoreoAppService.cs` |
| Permissions | `src/ExploraYa1.Application.Contracts/Permissions/ExploraYa1Permissions.cs` |
| Service | `src/ExploraYa1.Application/Monitoreo/MonitoreoAppService.cs` |
| Decorator | `src/ExploraYa1.Application/Monitoreo/ApiExternaLogDecorator.cs` |
| EF config + migration | `ExploraYa1DbContext.cs` + `20260408154931_AddApiExternaLogsTable.cs` |
| Controller | `src/ExploraYa1.HttpApi/Controllers/MonitoreoController.cs` |
| Tests | `test/ExploraYa1.Application.Tests/Monitoreo/` |

**API endpoint:**
- `GET /api/app/monitoreo/metricas?desde={datetime}&hasta={datetime}` — returns aggregated metrics per external API (requires `ExploraYa1.Monitoreo` permission / admin role)

**How logging works:** `ApiExternaLogDecorator` wraps `ICitySearchService`. Every call to the GeoDB API inserts a row in `AppApiExternaLogs` via `finally` block — even on failure. DB write errors are silently swallowed (logged via `ILogger`).

**Permissions:** `ExploraYa1Permissions.Monitoreo.Default = "ExploraYa1.Monitoreo"`. Granted to the `admin` role by `MonitoreoDataSeedContributor` (uses string literal — Domain can't reference Application.Contracts).

**After cloning / DB reset:** run `dotnet run --project src/ExploraYa1.DbMigrator` to apply `AppApiExternaLogs` migration and seed the admin permission.
