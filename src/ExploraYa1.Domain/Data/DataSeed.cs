using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using ExploraYa1.Destinos;

namespace ExploraYa1.Data;

// IDs fijos para tests/aserciones deterministas (opcional pero recomendable)
public static class RegionSeedIds
{
    public static readonly Guid UruguayId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // País
    public static readonly Guid LitoralNorteId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid CentroSurId = Guid.Parse("33333333-3333-3333-3333-333333333333");
}

public class RegionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Pais, Guid> _paisRepo;
    private readonly IRepository<Region, Guid> _regionRepo;
    private readonly IGuidGenerator _guid;

    public RegionDataSeedContributor(
        IRepository<Pais, Guid> paisRepo,
        IRepository<Region, Guid> regionRepo,
        IGuidGenerator guid)
    {
        _paisRepo = paisRepo;
        _regionRepo = regionRepo;
        _guid = guid;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1) País base (si Region depende de País en tu modelo, deja esto; si no, puedes omitirlo)
        var uruguay = await _paisRepo.FindAsync(RegionSeedIds.UruguayId);
        if (uruguay is null)
        {
            uruguay = await _paisRepo.InsertAsync(new Pais
            {
               // Id = RegionSeedIds.UruguayId,
                Nombre = "Uruguay"
            }, autoSave: true);
        }

        // 2) Regiones (no duplicar si ya existen)
        if (!await _regionRepo.AnyAsync(r => r.Id == RegionSeedIds.LitoralNorteId))
        {
            await _regionRepo.InsertAsync(new Region
            {
               // Id = RegionSeedIds.LitoralNorteId,
                Nombre = "Litoral Norte",
                Descripcion = "Región del litoral norte uruguayo",
                // IdPais = uruguay.Id, // ← si tu Region tiene FK a País, descomenta
            }, autoSave: true);
        }

        if (!await _regionRepo.AnyAsync(r => r.Id == RegionSeedIds.CentroSurId))
        {
            await _regionRepo.InsertAsync(new Region
            {
                //Id = RegionSeedIds.CentroSurId,
                Nombre = "Centro Sur",
                Descripcion = "Región centro-sur uruguaya",
                // IdPais = uruguay.Id,
            }, autoSave: true);
        }
    }
}

