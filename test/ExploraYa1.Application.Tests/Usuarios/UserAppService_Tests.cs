using ExploraYa1.UserProfiles;
using ExploraYa1.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace ExploraYa1.Usuarios
{
    public class UserAppService_Tests
    {
        private readonly AutoMocker _mocker;

        private readonly Mock<IdentityUserManager> _userManagerMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
        private readonly Mock<Volo.Abp.ObjectMapping.IObjectMapper> _objectMapperMock;
        private readonly Mock<IGuidGenerator> _guidGeneratorMock;
        private Mock<IdentityUserManager> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<IdentityUser>>();

            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());

            var pwdHasher = new Mock<IPasswordHasher<IdentityUser>>();
            pwdHasher.Setup(x => x.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                     .Returns("HASHED");

            var userValidators = new List<IUserValidator<IdentityUser>>();
            var pwdValidators = new List<IPasswordValidator<IdentityUser>>();

            var normalizer = new Mock<ILookupNormalizer>();
            normalizer.Setup(n => n.NormalizeName(It.IsAny<string>()))
                      .Returns((string s) => s?.ToUpper());
            normalizer.Setup(n => n.NormalizeEmail(It.IsAny<string>()))
                      .Returns((string s) => s?.ToUpper());

            var describer = new IdentityErrorDescriber();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<IdentityUser>>>();

            var manager = new Mock<IdentityUserManager>(
                store.Object,
                options.Object,
                pwdHasher.Object,
                userValidators,
                pwdValidators,
                normalizer.Object,
                describer,
                services.Object,
                logger.Object
            );

            return manager;
        }

        private UserManager<IdentityUser> BuildRealUserManager()
{
    var store = new Mock<IUserStore<IdentityUser>>();

    var options = new Mock<IOptions<IdentityOptions>>();
    options.Setup(o => o.Value).Returns(new IdentityOptions());

    var passwordHasher = new PasswordHasher<IdentityUser>();

    var userValidators = new List<IUserValidator<IdentityUser>> { new UserValidator<IdentityUser>() };
    var pwdValidators = new List<IPasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() };

    return new UserManager<IdentityUser>(
        store.Object,
        options.Object,
        passwordHasher,
        userValidators,
        pwdValidators,
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        null,
        new Mock<ILogger<UserManager<IdentityUser>>>().Object
    );
}





        private readonly UserAppService _userAppService;

        // Propiedades para IDs consistentes en tests que crean entidades (RegisterAsync)
        private readonly Guid _testUserId = Guid.Parse("A0000000-0000-0000-0000-000000000001");
        private readonly Guid _testUserProfileId = Guid.Parse("A0000000-0000-0000-0000-000000000002");



            
        public UserAppService_Tests()
        {
            // -----------------------------------------------------------
            // CONSTRUCTOR CORREGIDO: INYECCIÓN UNIVERSAL POR HIERARCHY
            // -----------------------------------------------------------

            _mocker = new AutoMocker();

            // 1. Obtener los Mocks necesarios
            _userManagerMock = _mocker.GetMock<IdentityUserManager>();
            _currentUserMock = _mocker.GetMock<ICurrentUser>();
            _userProfileRepositoryMock = _mocker.GetMock<IUserProfileRepository>();
            _objectMapperMock = _mocker.GetMock<Volo.Abp.ObjectMapping.IObjectMapper>();
            _guidGeneratorMock = _mocker.GetMock<IGuidGenerator>();

            // 2. Usar Use<T>() para asegurar que el mock esté en el contexto de DI
            _mocker.Use<Volo.Abp.ObjectMapping.IObjectMapper>(_objectMapperMock.Object);

            // 3. Configurar la secuencia de creación de Guids
            _guidGeneratorMock.SetupSequence(x => x.Create())
                .Returns(_testUserId)
                .Returns(_testUserProfileId);

            // 4. Crear el AppService
            _userAppService = _mocker.CreateInstance<UserAppService>();

            // 🛑 SOLUCIÓN DEFINITIVA Y AGRESIVA (BindingFlags.FlattenHierarchy):
            // Busca campos privados del tipo IObjectMapper en TODA la jerarquía de herencia de AppService.
            var objectMapperField = typeof(UserAppService)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                .FirstOrDefault(f => f.FieldType == typeof(Volo.Abp.ObjectMapping.IObjectMapper));

            objectMapperField?.SetValue(_userAppService, _objectMapperMock.Object);
       
        
            
        
        
        }

        // -----------------------------------
        // PRUEBAS DE FLUJO POSITIVO Y NEGATIVO
        // -----------------------------------

        [Fact]
        public async Task LoginAsync_Should_Return_User_When_Credentials_Are_Valid()
        {
            // Arrange
            var input = new LoginUserDto
            {
                UserNameOrEmail = "testuser",
                Password = "Test@1234"
            };

            var user = new IdentityUser(Guid.NewGuid(), "testuser", "test@example.com");

            _userManagerMock.Setup(x => x.FindByNameAsync(input.UserNameOrEmail))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(input.UserNameOrEmail))
                .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, input.Password))
                .ReturnsAsync(true);

            // Act
            var result = await _userAppService.LoginAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.UserName.ShouldBe(user.UserName);
            result.Email.ShouldBe(user.Email);
        }

        [Fact]
        public async Task GetProfileAsync_Should_Return_User_Profile()
        {
            // Arrange
            var userId = _testUserId;
            var user = new IdentityUser(userId, "testuser", "test@example.com");
            var userProfile = new UserProfile(Guid.NewGuid(), userId)
            {
                Nombre = "Test",
                Apellido = "User"
            };

            _currentUserMock.Setup(x => x.Id).Returns(userId);
            _userManagerMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userProfileRepositoryMock.Setup(x => x.FindByUserIdAsync(userId)).ReturnsAsync(userProfile);

            // Act
            var result = await _userAppService.GetProfileAsync();

            // Assert
            result.ShouldNotBeNull();
            result.UserName.ShouldBe(user.UserName);
            result.Email.ShouldBe(user.Email);
            result.Nombre.ShouldBe(userProfile.Nombre);
            result.Apellido.ShouldBe(userProfile.Apellido);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_Update_User_And_Profile()
        {
            // Arrange
            var userId = _testUserId;
            var input = new UpdateUserProfileDto
            {
                Nombre = "Updated",
                Apellido = "User",
                Email = "updated@example.com",
                Telefono = "123456789",
                FotoUrl = "profile.jpg"
            };

            var user = new IdentityUser(userId, "testuser", "test@example.com");
            var userProfile = new UserProfile(Guid.NewGuid(), userId);

            _currentUserMock.Setup(x => x.Id).Returns(userId);

            _userManagerMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock
        .Setup(x => x.SetEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback<IdentityUser, string>((u, mail) =>
            {
             typeof(IdentityUser)
                    .GetProperty("Email")
            .SetValue(u, mail);
                 })
                .ReturnsAsync(IdentityResult.Success);


            // Moq debe ser flexible para aceptar la instancia modificada:
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(IdentityResult.Success);
            _userProfileRepositoryMock.Setup(x => x.FindByUserIdAsync(userId)).ReturnsAsync(userProfile);

            _userProfileRepositoryMock.Setup(x => x.UpdateAsync(userProfile, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((UserProfile p, bool a, CancellationToken c) => Task.FromResult(p));

            // Act
            var result = await _userAppService.UpdateProfileAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Nombre.ShouldBe(input.Nombre);
            result.Apellido.ShouldBe(input.Apellido);
            result.Email.ShouldBe(input.Email);
            result.Telefono.ShouldBe(input.Telefono);
            result.FotoUrl.ShouldBe(input.FotoUrl);

            _userProfileRepositoryMock.Verify(x => x.UpdateAsync(userProfile, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _userManagerMock.Verify(x => x.SetEmailAsync(user, input.Email), Times.Once);
            // Verificar que UpdateAsync fue llamado con el usuario modificado
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<IdentityUser>(u => u.Name == input.Nombre && u.Surname == input.Apellido)), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Change_Password()
        {
            // Arrange
            var userId = _testUserId;
            var input = new ChangePasswordDto
            {
                CurrentPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var user = new IdentityUser(userId, "testuser", "test@example.com");

            _currentUserMock.Setup(x => x.Id).Returns(userId);
            _userManagerMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userAppService.ChangePasswordAsync(input);

            // Assert
            _userManagerMock.Verify(x => x.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword), Times.Once);
        }

        [Fact]
        public async Task DeleteMyAccountAsync_Should_Delete_User()
        {
            // Arrange
            var userId = _testUserId;
            var user = new IdentityUser(userId, "testuser", "test@example.com");

            _currentUserMock.Setup(x => x.Id).Returns(userId);
            _userManagerMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userAppService.DeleteMyAccountAsync();

            // Assert
            _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetPublicProfileAsync_Should_Return_Public_Profile()
        {
            // Arrange
            var userId = _testUserId;
            var user = new IdentityUser(userId, "testuser", "test@example.com");
            var userProfile = new UserProfile(Guid.NewGuid(), userId)
            {
                Nombre = "Test",
                Apellido = "User",
                Telefono = "123456789",
                FotoUrl = "profile.jpg"
            };

            _userManagerMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userProfileRepositoryMock.Setup(x => x.FindByUserIdAsync(userId)).ReturnsAsync(userProfile);

            // Simular el ObjectMapper
            _objectMapperMock.Setup(
                x => x.Map<IdentityUser, UserProfileDto>(It.IsAny<IdentityUser>()))
                .Returns((IdentityUser source) =>
                {
                    // Asumiendo que el DTO público solo necesita estos campos del IdentityUser
                    return new UserProfileDto
                    {
                        Id = source.Id,
                        UserName = source.UserName,
                        Email = null // El email no se expone en el perfil público
                    };
                });


            // Act
            var result = await _userAppService.GetPublicProfileAsync(userId);

            // Assert
            result.ShouldNotBeNull();
            result.UserName.ShouldBe(user.UserName);
            result.Nombre.ShouldBe(userProfile.Nombre);
            result.Apellido.ShouldBe(userProfile.Apellido);
            result.Telefono.ShouldBe(userProfile.Telefono);
            result.FotoUrl.ShouldBe(userProfile.FotoUrl);
            result.Email.ShouldBe(null); // Verificamos que el email no se haya filtrado
        }

        // -----------------------------------
        // PRUEBAS DE FLUJO NEGATIVO
        // -----------------------------------

        [Fact]
        public async Task LoginAsync_Should_Throw_If_User_Not_Found()
        {
            // Arrange
            var input = new LoginUserDto
            {
                UserNameOrEmail = "nonexistent",
                Password = "Password"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(input.UserNameOrEmail))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(input.UserNameOrEmail))
                .ReturnsAsync((IdentityUser)null);

            // Act & Assert
            await Should.ThrowAsync<UserFriendlyException>(
                () => _userAppService.LoginAsync(input)
            );

            _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_If_Password_Is_Invalid()
        {
            // Arrange
            var input = new LoginUserDto
            {
                UserNameOrEmail = "testuser",
                Password = "WrongPassword"
            };

            var user = new IdentityUser(Guid.NewGuid(), "testuser", "test@example.com");

            _userManagerMock.Setup(x => x.FindByNameAsync(input.UserNameOrEmail))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, input.Password))
                .ReturnsAsync(false);

            // Act & Assert
            await Should.ThrowAsync<UserFriendlyException>(
                () => _userAppService.LoginAsync(input)
            );

            _userManagerMock.Verify(x => x.CheckPasswordAsync(user, input.Password), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_UserFriendlyException_If_User_Creation_Fails()
        {
            var input = new RegisterUserDto
            {
                UserName = "existinguser",
                Email = "existing@example.com",
                Password = "Test@1234"
            };

            // Hace que falle la creación del usuario (lo que queremos testear)
            _userManagerMock.Setup(x => x.CreateAsync(
                It.IsAny<IdentityUser>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Description = "Username already taken." }
                ));

            // Normalización necesaria
            _userManagerMock.Setup(x => x.NormalizeName(It.IsAny<string>()))
                .Returns<string>(n => n?.ToUpper());

            _userManagerMock.Setup(x => x.NormalizeEmail(It.IsAny<string>()))
                .Returns<string>(e => e?.ToUpper());

            // UserManager chequea existencia antes de crear
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(x => x.SupportsUserEmail)
                .Returns(true);

            // 🔥 Estos son los mocks que te faltaban
            _userManagerMock.Setup(x => x.SetEmailAsync(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.SetUserNameAsync(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Verificación: NO se debe insertar perfil porque CreateAsync falla
            _userProfileRepositoryMock
                .Setup(x => x.InsertAsync(
                        It.IsAny<UserProfile>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()))
                .Throws(new Exception("NO debería llamarse"));

            // ASSERT
            // Ya tenés SetUserNameAsync y SetEmailAsync (los dejamos igual)
            _userManagerMock.Setup(x => x.SetUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.SetEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // FALTABA ESTE: se usa siempre antes de CreateAsync
            _userManagerMock.Setup(x => x.UpdateSecurityStampAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // FALTABA: ABP valida el usuario
            _userManagerMock.Setup(x => x.CallValidateUserAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // FALTABA: ABP valida contraseña ANTES de CreateAsync
            _userManagerMock.Setup(x => x.CallValidatePasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // FALTABA: ABP intenta asignar roles después de crear
            _userManagerMock.Setup(x => x.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            // Necesario porque internamente llama Options
            _userManagerMock.SetupGet(x => x.Options)
                .Returns(new IdentityOptions());

            // Esto evita NRE por PasswordHasher nulo
            _userManagerMock.SetupGet(x => x.PasswordHasher)
                .Returns(new PasswordHasher<IdentityUser>());






            await Should.ThrowAsync<UserFriendlyException>(() =>
                _userAppService.RegisterAsync(input)
            );

            _userProfileRepositoryMock.Verify(x =>
                x.InsertAsync(
                    It.IsAny<UserProfile>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }



        [Fact]
        public async Task UpdateProfileAsync_Should_Throw_If_UserProfile_Is_Missing_And_Cant_Create()
        {
            // Arrange
            var userId = _testUserId;
            var input = new UpdateUserProfileDto { Nombre = "NewName" };

            // Mock correcto: ICurrentUser.Id
            _currentUserMock.Setup(x => x.Id).Returns(userId);

            // 1. Simular que el IdentityUser existe
            var user = new IdentityUser(userId, "testuser", "test@example.com");
            _userManagerMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // 2. Simular que el UserProfile NO existe
            _userProfileRepositoryMock.Setup(x => x.FindByUserIdAsync(userId))
                .ReturnsAsync((UserProfile)null);

            // Simular fallo al crear/insertar el perfil
            _userProfileRepositoryMock.Setup(x =>
                x.InsertAsync(It.IsAny<UserProfile>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("Fallo de persistencia simulado."));

            // Act & Assert
            await Should.ThrowAsync<System.Exception>(() =>
                _userAppService.UpdateProfileAsync(input)
            );

            // Debe fallar antes de actualizar el IdentityUser, así que nunca se llama UpdateAsync
            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<IdentityUser>()), Times.Never);

            // InsertAsync se intentó exactamente una vez
            _userProfileRepositoryMock.Verify(x =>
                x.InsertAsync(It.IsAny<UserProfile>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

    }
}