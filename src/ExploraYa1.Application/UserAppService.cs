
using ExploraYa1.UserProfiles;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.ObjectMapping;



namespace ExploraYa1.Usuarios
{
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentUser _currentUser;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IObjectMapper _mapper;

        public UserAppService(
        IdentityUserManager userManager,
        ICurrentUser currentUser,
        IUserProfileRepository userProfileRepository,
        IObjectMapper mapper)    
        {
            _userManager = userManager;
            _currentUser = currentUser;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper; 
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

            // --- NUEVA LÓGICA DE REGISTRO DE PERFIL ---
            // Creamos un UserProfile por defecto para el nuevo usuario
            var userProfile = new UserProfile(GuidGenerator.Create(), user.Id);


            userProfile.Nombre = input.UserName; // Usamos el UserName como Nombre inicial
            userProfile.Apellido = string.Empty;
            userProfile.Telefono = string.Empty;
            userProfile.FotoUrl = string.Empty;
            

            await _userProfileRepository.InsertAsync(userProfile);
            // ------------------------------------------

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Nombre = userProfile.Nombre,
                Apellido = userProfile.Apellido,
                Telefono = userProfile.Telefono,
                FotoUrl = userProfile.FotoUrl
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
            var userId = _currentUser.GetId();
            var identityUser = await _userManager.GetByIdAsync(userId);
            var userProfile = await _userProfileRepository.FindByUserIdAsync(userId); // <-- Obtener el perfil

            var dto = new UserProfileDto
            {
                Id = identityUser.Id,
                Email = identityUser.Email,
                UserName = identityUser.UserName,
                // Mapeo de datos del UserProfile
                Nombre = userProfile?.Nombre ?? string.Empty,
                Apellido = userProfile?.Apellido ?? string.Empty,
                Telefono = userProfile?.Telefono ?? string.Empty,
                FotoUrl = userProfile?.FotoUrl ?? string.Empty
            };
            return dto;
        }

        // ============================================
        // 4) UPDATE PROFILE
        // ============================================
        public async Task<UserProfileDto> UpdateProfileAsync(UpdateUserProfileDto input)
        {
            // 1. Validar Autorización (Solo el usuario logueado)
            Guid? userId = _currentUser.GetId();


            // Si el usuario no está autenticado, userId.HasValue será false.
            // Aunque [Authorize] lo previene, esta validación es útil y necesaria si el tipo es Guid?.
            if (!userId.HasValue)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para actualizar su perfil.");
            }

            var currentUserId = userId.Value;

            // 2. Obtener IdentityUser (de dbo.AppUsers)
            var identityUser = await _userManager.GetByIdAsync(currentUserId);
            if (identityUser == null)
            {
                throw new EntityNotFoundException(typeof(Volo.Abp.Identity.IdentityUser), currentUserId);
            }

            // 3. Obtener o Crear UserProfile (de dbo.AppUserProfiles)
            var userProfile = await _userProfileRepository.FindByUserIdAsync(currentUserId);

            // FIX DE ROBUSTEZ: Si el perfil personalizado no existe, lo creamos
            if (userProfile == null)
            {
                userProfile = new UserProfile(Guid.NewGuid(), currentUserId);

                // Inicializamos Nombre/Apellido del perfil al crear
                userProfile.Nombre = input.Nombre;
                userProfile.Apellido = input.Apellido;

                userProfile = await _userProfileRepository.InsertAsync(userProfile, autoSave: true);
            }

            // =======================================================
            // A) ACTUALIZAR IDENTITYUSER
            // =======================================================

            // Asignamos Nombre/Apellido a la tabla principal (dbo.AppUsers)
            identityUser.Name = input.Nombre;
            identityUser.Surname = input.Apellido;

            // 🛑 LÓGICA ELIMINADA: BLOQUEO DEL CAMBIO DE USERNAME
            // Ya no se llama a SetUserNameAsync. El UserName se mantiene inmutable
            // para asegurar la trazabilidad y la identificación única de la cuenta.
            /*
            if (!string.IsNullOrWhiteSpace(input.UserName) && identityUser.UserName != input.UserName)
                await _userManager.SetUserNameAsync(identityUser, input.UserName);
            */

            // ✅ SÍ SE PERMITE: Actualizar Email si cambió
            if (!string.IsNullOrWhiteSpace(input.Email) && identityUser.Email != input.Email)
                await _userManager.SetEmailAsync(identityUser, input.Email);

            var identityResult = await _userManager.UpdateAsync(identityUser);
            if (!identityResult.Succeeded)
                throw new UserFriendlyException(identityResult.Errors.First().Description);


            // =======================================================
            // B) ACTUALIZAR USERPROFILE
            // =======================================================

            // Nota: Estos campos ya se inicializaron si el perfil se creó, y se actualizan aquí si existe
            userProfile.Nombre = input.Nombre;
            userProfile.Apellido = input.Apellido;
            userProfile.Telefono = input.Telefono;
            userProfile.FotoUrl = input.FotoUrl;

            await _userProfileRepository.UpdateAsync(userProfile);

            // 4. Devolver DTO combinado
            return new UserProfileDto
            {
                Id = identityUser.Id,
                UserName = identityUser.UserName, // Retorna el UserName original
                Email = identityUser.Email,

                // Datos del perfil extendido
                Nombre = userProfile.Nombre,
                Apellido = userProfile.Apellido,
                Telefono = userProfile.Telefono,
                FotoUrl = userProfile.FotoUrl
            };
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
            // 1. Obtener los datos estándar del usuario (de dbo.AppUsers)
            var user = await _userManager.GetByIdAsync(userId);
            if (user == null)
            {
                // Lanza una excepción si el usuario no existe.
                throw new EntityNotFoundException(typeof(Volo.Abp.Identity.IdentityUser), userId);
            }

            // 2. Obtener el perfil personalizado (de dbo.AppUserProfiles)
            //    Usamos el método FindByUserIdAsync que implementaste en el repositorio.
            var profile = await _userProfileRepository.FindByUserIdAsync(userId);

            // 3. Mapear y combinar los datos en el DTO
            var userDto = _mapper.Map<IdentityUser, UserProfileDto>(user);


            if (profile != null)
            {
                // Mapear los campos personalizados si el perfil existe
                userDto.Nombre = profile.Nombre;
                userDto.Apellido = profile.Apellido;
                userDto.Telefono = profile.Telefono;
                userDto.FotoUrl = profile.FotoUrl;
            }
            else
            {
                // Si el perfil no se encontró, puedes mapear los campos 'Name' y 'Surname'
                // de IdentityUser al DTO como respaldo.
                userDto.Nombre = user.Name;
                userDto.Apellido = user.Surname;
            }

            return userDto;
        }
    }
}

