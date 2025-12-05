using ExploraYa1.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Moq;
using NSubstitute;
using Shouldly; // Usado para aserciones limpias (ej. .ShouldNotBeNull())
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

// Nota: Debes ajustar el namespace según la ubicación real de tus tests
namespace ExploraYa1.Usuarios
{
    // Asegúrate de que esta clase hereda de tu ExploraYa1ApplicationTestBase
    public class UserAppService_Tests : ExploraYa1ApplicationTestBase<ExploraYa1ApplicationTestModule>
    {
        // Declaramos los Mocks para las dependencias del AppService
        private readonly UserAppService _userAppService;
        private readonly Mock<IdentityUserManager> _mockUserManager;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepository;
        private readonly Mock<ICurrentUser> _mockCurrentUser;

        public UserAppService_Tests()
        {
            // 1. Obtener y configurar los Mocks
            // En un módulo de pruebas de ABP, las dependencias se reemplazan con Mocks,
            // y luego se obtienen aquí.
            _mockUserManager = GetRequiredService<Mock<IdentityUserManager>>();
            _mockUserProfileRepository = GetRequiredService<Mock<IUserProfileRepository>>();
            _mockCurrentUser = GetRequiredService<Mock<ICurrentUser>>();

            // 2. Obtener la instancia del servicio de aplicación que usa los Mocks
            _userAppService = GetRequiredService<UserAppService>();
        }

        // =================================================================
        // TEST 1: RegisterAsync
        // =================================================================
        [Fact]
        public async Task RegisterAsync_ShouldCreateIdentityUserAndUserProfile()
        {
            // Arrange
            // Usamos una variable para CAPTURAR el ID que el servicio genera y usa.
            Guid actualUserId = Guid.Empty;

            var registerDto = new RegisterUserDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123"
            };

            // Simular la creación exitosa del IdentityUser
            _mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success)

                // 🚨 FIX CS0272: Leemos el Id que la lógica del servicio ya asignó
                // al IdentityUser (user) antes de que fuera pasado al mock.
                .Callback<IdentityUser, string>((user, pass) => actualUserId = user.Id);
            // -------------------------------------------------------------

            // Act
            var result = await _userAppService.RegisterAsync(registerDto);

            // Assert
            result.ShouldNotBeNull();
            result.UserName.ShouldBe(registerDto.UserName);
            result.Nombre.ShouldBe(registerDto.UserName);

            // Verificar que el repositorio fue llamado para insertar el perfil,
            // usando el ID que fue capturado del objeto IdentityUser.
            _mockUserProfileRepository.Verify(
                x => x.InsertAsync(
                    // Utilizamos el ID CAPTURADO para la verificación.
                    It.Is<UserProfile>(p => p.UserId == actualUserId && p.Nombre == registerDto.UserName),
                    true, // autoSave: true (asumimos true o el default del repositorio)
                    CancellationToken.None),
                Times.Once);
        }

        // =================================================================
        // TEST 2: UpdateProfileAsync - Inmutabilidad y Actualización de Email
        // =================================================================
        [Fact]
        public async Task UpdateProfileAsync_ShouldUpdateProfileButIgnoreUserNameChange()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalUserName = "oldusername";
            var originalEmail = "old@email.com";
            var newEmail = "newemail@test.com";

            // Entidades de la base de datos
            var identityUser = new IdentityUser(userId, originalUserName, originalEmail) { Name = "Old", Surname = "User" };
            var existingProfile = new UserProfile(Guid.NewGuid(), userId) { Nombre = "Old", Apellido = "Profile" };

            var updateDto = new UpdateUserProfileDto
            {
                // Este cambio de UserName DEBE SER IGNORADO por la lógica de tu AppService
                UserName = "hacker_attempt",
                Email = newEmail,
                Nombre = "New",
                Apellido = "Name",
                Telefono = "123456789",
                FotoUrl = "http://new.photo.url"
            };

            // 1. Configurar el CurrentUser logueado
            _mockCurrentUser.Setup(x => x.GetId()).Returns(userId);

            // 2. Configurar IdentityUserManager
            _mockUserManager.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(identityUser);
            _mockUserManager.Setup(x => x.SetEmailAsync(identityUser, newEmail)).Returns((Task<IdentityResult>)Task.CompletedTask);
            _mockUserManager.Setup(x => x.UpdateAsync(identityUser)).ReturnsAsync(IdentityResult.Success);

            // 3. Configurar Repositorio de Perfil (Existe)
            _mockUserProfileRepository.Setup(x => x.FindByUserIdAsync(userId)).ReturnsAsync(existingProfile);

            // Act
            var result = await _userAppService.UpdateProfileAsync(updateDto);

            // Assert
            result.ShouldNotBeNull();

            // 🛑 VERIFICAR INMUTABILIDAD: El UserName devuelto DEBE ser el original
            result.UserName.ShouldBe(originalUserName);

            // Verificar que el Email sí se actualizó
            result.Email.ShouldBe(newEmail);

            // Verificar que los nombres en el DTO de salida son los nuevos
            result.Nombre.ShouldBe(updateDto.Nombre);

            // Verificar que SetUserNameAsync NUNCA fue llamado
            _mockUserManager.Verify(x => x.SetUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }


        // =================================================================
        // TEST 3: UpdateProfileAsync - Robustez (Perfil NO Existe, Debe Crear)
        // =================================================================
        [Fact]
        public async Task UpdateProfileAsync_ShouldCreateProfile_WhenProfileDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // El IdentityUser que recupera el servicio. Se le asignarán Name/Surname.
            var identityUser = new IdentityUser(userId, "username", "email@test.com");

            var updateDto = new UpdateUserProfileDto
            {
                Nombre = "New",
                Apellido = "Profile",
                Telefono = "123456789" // Agregamos un campo para verificar la actualización
            };

            // --- Setup de Mocks ---

            // 1. Configurar el CurrentUser logueado
            _mockCurrentUser.Setup(x => x.GetId()).Returns(userId);

            // 2. Configurar IdentityUserManager
            _mockUserManager.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(identityUser);
            _mockUserManager.Setup(x => x.UpdateAsync(identityUser)).ReturnsAsync(IdentityResult.Success);

            // 3. Configurar Repositorio de Perfil (NO Existe: retorna null)
            _mockUserProfileRepository.Setup(x => x.FindByUserIdAsync(userId)).ReturnsAsync((UserProfile)null);

            // 4. Simular la INSERCIÓN (InsertAsync)
            _mockUserProfileRepository
                .Setup(x => x.InsertAsync(It.IsAny<UserProfile>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))

                // 🚨 FIX: Usamos la sobrecarga de Returns con 3 tipos genéricos (Argumentos del método)
                .Returns<UserProfile, bool, CancellationToken>((profile, autoSave, token) =>
                    Task.FromResult(profile));
            // El lambda recibe los 3 argumentos y devuelve Task<UserProfile>

            // 5. Simular la ACTUALIZACIÓN (UpdateAsync)
            _mockUserProfileRepository
                .Setup(x => x.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))

                // 🚨 FIX: Mismo patrón
                .Returns<UserProfile, bool, CancellationToken>((profile, autoSave, token) =>
                    Task.FromResult(profile));


            // Act
            var result = await _userAppService.UpdateProfileAsync(updateDto);

            // Assert
            result.ShouldNotBeNull();
            result.Nombre.ShouldBe(updateDto.Nombre);
            result.Telefono.ShouldBe(updateDto.Telefono);

            // --- Verificaciones (Verify) ---

            // 1. Verificar que se llamó a INSERTAsync (para crear el perfil que faltaba)
            _mockUserProfileRepository.Verify(
                x => x.InsertAsync(
                    It.Is<UserProfile>(p =>
                        p.UserId == userId &&
                        p.Nombre == updateDto.Nombre), // Verifica que el Nombre inicial se asignó al crear
                    true, CancellationToken.None),
                Times.Once);

            // 2. Verificar que se llamó a UPDATEAsync
            // Este servicio llama a InsertAsync (crear) y luego a UpdateAsync (para el resto de los campos).
            // Verificamos que se llamó a UpdateAsync con los datos actualizados.
            _mockUserProfileRepository.Verify(
                x => x.UpdateAsync(
                    It.Is<UserProfile>(p =>
                        p.UserId == userId &&
                        p.Telefono == updateDto.Telefono), // Verifica que la actualización ocurrió
                    It.IsAny<bool>(), CancellationToken.None),
                Times.Once);


            // 3. Verificar que la tabla principal (IdentityUser) se actualizó correctamente
            identityUser.Name.ShouldBe(updateDto.Nombre);
            identityUser.Surname.ShouldBe(updateDto.Apellido);
            _mockUserManager.Verify(x => x.UpdateAsync(identityUser), Times.Once);
        }

        // =================================================================
        // TEST 4: UpdateProfileAsync - Falla de Autorización (Seguridad)
        // =================================================================
        [Fact]
        public async Task UpdateProfileAsync_ShouldThrowException_WhenNotAuthenticated()
        {
            // Arrange
            var updateDto = new UpdateUserProfileDto();

            // 1. Configurar el CurrentUser como NO logueado (Devuelve GUID? nulo)
            // Cuando el código del servicio llama a _currentUser.GetId(), obtendrá null.
            _mockCurrentUser.Setup(x => x.GetId()).Returns((Guid)(Guid?)null);

            // Act & Assert
            // Se espera que el servicio lance AbpAuthorizationException 
            // al intentar acceder al ID nulo o al inicio del método.
            await Should.ThrowAsync<AbpAuthorizationException>(
                _userAppService.UpdateProfileAsync(updateDto)
            );
        }
    }
}