using ExploraYa1.Usuarios;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ExploraYa1
{
    public interface IUserAppService : IApplicationService
    {
        Task<UserProfileDto> RegisterAsync(RegisterUserDto input);
        Task<UserProfileDto> LoginAsync(LoginUserDto input);
        Task<UserProfileDto> GetProfileAsync();

        Task ChangePasswordAsync(ChangePasswordDto input);
        Task DeleteMyAccountAsync();
        Task<UserProfileDto> GetPublicProfileAsync(Guid userId);

    }
}

