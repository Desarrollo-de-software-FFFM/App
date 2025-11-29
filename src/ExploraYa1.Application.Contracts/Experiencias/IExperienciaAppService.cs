using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Experiencias
{
    public interface IExperienciaAppService : IApplicationService
    {
        Task<ExperienciaDto> CreateAsync(CrearActualizarExperienciaDto input);
        Task<ExperienciaDto> UpdateAsync(Guid id, CrearActualizarExperienciaDto input);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<ExperienciaDto>> GetListAsync(GetExperienciasInput input);
    }
}
