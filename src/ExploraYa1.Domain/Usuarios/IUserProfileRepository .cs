using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.UserProfiles
{
    public interface IUserProfileRepository : IRepository<UserProfile, Guid>
    {
        Task<UserProfile> FindByUserIdAsync(Guid userId);
    }
}
