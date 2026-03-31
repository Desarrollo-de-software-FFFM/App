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

### Frontend (Angular — run from `angular/` directory)
```bash
yarn install          # or npm install
ng serve              # Dev server at http://localhost:4200
ng build
ng test               # Karma unit tests
ng lint
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
- `Favorito` — user-saved destinations (links UserId ↔ DestinoTuristicoId)
- `CalificacionDestino` — user ratings for destinations
- `Region` / `Pais` — geographic hierarchy (country → region → destination)

**ABP patterns in use:**
- `CrudAppService<TEntity, TDto, TKey>` as base for standard CRUD services
- `IRepository<T, TKey>` for all data access — inject repositories, don't use DbContext directly in services
- AutoMapper for DTO ↔ entity mapping (mappings configured in `*ApplicationAutoMapperProfile`)
- ABP modules: each layer has a `*Module.cs` that registers dependencies and imports other modules

## Testing

Tests live in `test/`. The test base project (`ExploraYa1.TestBase`) provides shared fixtures.

- Domain tests: pure unit tests, no DB
- Application tests: use in-memory test doubles
- EF Core tests: integration tests against a real (LocalDB) database

## Key Configuration

`src/ExploraYa1.HttpApi.Host/appsettings.json`:
- API self URL: `https://localhost:44391`
- Angular URL / CORS: `http://localhost:4200`
- DB connection: `Server=(LocalDb)\MSSQLLocalDB;Database=ExploraYa1`
