using ExploraYa1.UserProfiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ExploraYa1.EntityFrameworkCore
{
    // ⬇️ CLAVE: Herencia de EfCoreRepository e Implementación de la interfaz
    public class EfCoreUserProfileRepository
        : EfCoreRepository<ExploraYa1DbContext, UserProfile, Guid>, // <--- Los tipos de TDbContext, TEntity, TKey deben ser correctos
          IUserProfileRepository
    {
        public EfCoreUserProfileRepository(
            IDbContextProvider<ExploraYa1DbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // El resto de los métodos de la interfaz deben estar aquí...
        public async Task<UserProfile> FindByUserIdAsync(Guid userId)
        {
            var dbSet = await GetDbSetAsync();

            // Usamos FirstOrDefaultAsync para buscar el perfil basado en el UserId
            return await dbSet.FirstOrDefaultAsync(profile => profile.UserId == userId);
        }


    }



}
