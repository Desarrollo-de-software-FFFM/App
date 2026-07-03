using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace ExploraYa1.Experiencias
{
    // [Authorize] asegura que solo usuarios logueados puedan usar estos métodos
    [Authorize]
    public class ExperienciaAppService : ApplicationService, IExperienciaAppService
    {
        private readonly IRepository<Experiencia, Guid> _repository;

        public ExperienciaAppService(IRepository<Experiencia, Guid> repository)
        {
            _repository = repository;
        }

        // 4.1 Crear una nueva experiencia
        public async Task<ExperienciaDto> CreateAsync(CrearActualizarExperienciaDto input)
        {
            var experiencia = new Experiencia(
                GuidGenerator.Create(),
                input.DestinoId,
                input.Comentario,
                input.Valoracion
            );

            await _repository.InsertAsync(experiencia);

            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experiencia);
        }

        // 4.2 Editar una experiencia (Solo si es propia)
        public async Task<ExperienciaDto> UpdateAsync(Guid id, CrearActualizarExperienciaDto input)
        {
            var experiencia = await _repository.GetAsync(id);

            // Validamos que el usuario actual sea el creador
            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("No puedes editar una experiencia que no es tuya.");
            }

            experiencia.Comentario = input.Comentario;
            experiencia.Valoracion = input.Valoracion;

            await _repository.UpdateAsync(experiencia);

            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experiencia);
        }

        // 4.3 Eliminar una experiencia (Solo si es propia)
        public async Task DeleteAsync(Guid id)
        {
            var experiencia = await _repository.GetAsync(id);

            // Validamos que el usuario actual sea el creador
            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("No puedes eliminar una experiencia de otro usuario.");
            }

            await _repository.DeleteAsync(id);
        }

        // 4.4, 4.5, 4.6 Consultar, Filtrar y Buscar
        public async Task<PagedResultDto<ExperienciaDto>> GetListAsync(GetExperienciasInput input)
        {
            // Obtener el queryable inicial
            var queryable = await _repository.GetQueryableAsync();

            // 1. Filtro obligatorio por Ciudad/Destino
            queryable = queryable.Where(x => x.DestinoId == input.DestinoId);

            // 2. Filtro opcional por Valoración (Punto 4.5)
            if (input.Valoracion.HasValue)
            {
                queryable = queryable.Where(x => x.Valoracion == input.Valoracion.Value);
            }

            // 3. Filtro opcional por Palabras Clave (Punto 4.6)
            if (!input.PalabrasClave.IsNullOrWhiteSpace())
            {
                // Busca si el comentario contiene la palabra
                queryable = queryable.Where(x => x.Comentario.Contains(input.PalabrasClave));
            }

            // Contar total de resultados (para la paginación)
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            // Ordenar (más recientes primero) y Paginar
            queryable = queryable
                .OrderByDescending(x => x.CreationTime)
                .PageBy(input);

            // Ejecutar consulta y obtener lista
            var experiencias = await AsyncExecuter.ToListAsync(queryable);

            // Convertir a DTO y devolver
            return new PagedResultDto<ExperienciaDto>(
                totalCount,
                ObjectMapper.Map<List<Experiencia>, List<ExperienciaDto>>(experiencias)
            );
        }
    }
}
