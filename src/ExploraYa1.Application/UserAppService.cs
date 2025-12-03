using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Users;


namespace ExploraYa1.Usuarios
{
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentUser _currentUser;

        public UserAppService(
            IdentityUserManager userManager,
            ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        // ============================================
        // 1) REGISTER
        // ============================================
        public async Task<UserProfileDto> RegisterAsync(RegisterUserDto input)
        {
            var user = new IdentityUser(
                GuidGenerator.Create(),
                input.UserName,
                input.Email
            );

            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
                throw new UserFriendlyException(result.Errors.First().Description);

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        // ============================================
        // 2) LOGIN (solo validación)
        // ============================================
        public async Task<UserProfileDto> LoginAsync(LoginUserDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserNameOrEmail)
                       ?? await _userManager.FindByEmailAsync(input.UserNameOrEmail);

            if (user == null)
                throw new UserFriendlyException("Usuario no encontrado");

            var valid = await _userManager.CheckPasswordAsync(user, input.Password);

            if (!valid)
                throw new UserFriendlyException("Contraseña incorrecta");

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        // ============================================
        // 3) GET PROFILE (usuario actual)
        // ============================================
        public async Task<UserProfileDto> GetProfileAsync()
        {
            var user = await _userManager.GetByIdAsync(_currentUser.GetId());

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        // ============================================
        // 4) UPDATE PROFILE
        // ============================================
        public async Task<UserProfileDto> UpdateProfileAsync(UpdateProfileDto input)
        {
            var userId = CurrentUser.GetId();
            var user = await _userManager.GetByIdAsync(userId);

            // ------------ PROPIEDADES ACCESIBLES -------------
            if (!string.IsNullOrWhiteSpace(input.Name))
                user.Name = input.Name;

            if (!string.IsNullOrWhiteSpace(input.Surname))
                user.Surname = input.Surname;

            // ------------ PROPIEDADES NO ACCESIBLES -------------
            if (!string.IsNullOrWhiteSpace(input.UserName))
                await _userManager.SetUserNameAsync(user, input.UserName);

            if (!string.IsNullOrWhiteSpace(input.Email))
                await _userManager.SetEmailAsync(user, input.Email);

            // ------------ GUARDAR CAMBIOS -------------
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new UserFriendlyException(result.Errors.First().Description);

            return ObjectMapper.Map<IdentityUser, UserProfileDto>(user);
        }

        // ============================================
        // 5) CAMBIAR PASSWORD
        // ============================================
        public async Task ChangePasswordAsync(ChangePasswordDto input)
        {
            var user = await _userManager.GetByIdAsync(_currentUser.Id.Value);

            var result = await _userManager.ChangePasswordAsync(
                user,
                input.CurrentPassword,
                input.NewPassword
            );

            if (!result.Succeeded)
                throw new UserFriendlyException(result.Errors.First().Description);
        }

        // ============================================
        // 6) DELETE ACCOUNT
        // ============================================
        public async Task DeleteMyAccountAsync()
        {
            var user = await _userManager.GetByIdAsync(_currentUser.Id.Value);

            await _userManager.DeleteAsync(user);
        }

        // ============================================
        // 7) PERFIL PÚBLICO
        // ============================================
        public async Task<UserProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userManager.GetByIdAsync(userId);

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                
            };
        }

        public Task UpdateUserProfileAsync(UpdateUserProfileDto input)
        {
            throw new NotImplementedException();
        }
    }
}

