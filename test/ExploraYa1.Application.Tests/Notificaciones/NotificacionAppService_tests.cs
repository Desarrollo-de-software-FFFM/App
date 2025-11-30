using ExploraYa1.Notificaciones;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace ExploraYa1.Notificaciones
{
    public class NotificacionAppService_tests
    {
        // -------------------------------------------------------------
        // 1) Crear notificación para un destino
        // -------------------------------------------------------------
        [Fact]
        public async Task CrearNotificacionCambioDestinoAsync_ShouldInsertNotification()
        {
            var mockNotifRepo = Substitute.For<IRepository<Notificacion, Guid>>();
            var mockCurrentUser = Substitute.For<ICurrentUser>();

            var fakeUserId = Guid.NewGuid();
            mockCurrentUser.Id.Returns(fakeUserId);

            var service = new NotificacionAppService(mockNotifRepo, mockCurrentUser);

            var destinoId = Guid.NewGuid();

            await service.CrearNotificacionCambioDestinoAsync(destinoId, "Detalle test");

            await mockNotifRepo.Received(1).InsertAsync(
                Arg.Is<Notificacion>(n =>
                    n.UserId == fakeUserId &&
                    n.DestinoId == destinoId &&
                    n.Leida == false &&
                    n.Mensaje == "Detalle test"
                )
            );
        }

        // -------------------------------------------------------------
        // 2) Marcar como leída
        // -------------------------------------------------------------
        [Fact]
        public async Task MarcarLeidaAsync_ShouldMarkAsRead()
        {
            var notifId = Guid.NewGuid();

            var notif = new Notificacion(Guid.NewGuid())
            {
               
                Leida = false
            };

            var mockNotifRepo = Substitute.For<IRepository<Notificacion, Guid>>();
            mockNotifRepo.GetAsync(notifId).Returns(notif);

            var mockCurrentUser = Substitute.For<ICurrentUser>();

            var service = new NotificacionAppService(mockNotifRepo, mockCurrentUser);

            await service.MarcarLeidaAsync(notifId);

            notif.Leida.ShouldBeTrue();
            await mockNotifRepo.Received(1).UpdateAsync(notif);
        }

        // -------------------------------------------------------------
        // 3) Marcar como no leída
        // -------------------------------------------------------------
        [Fact]
        public async Task MarcarNoLeidaAsync_ShouldMarkAsUnRead()
        {
            var notifId = Guid.NewGuid();

            var notif = new Notificacion(Guid.NewGuid())
            {
                
                Leida = true
            };

            var mockNotifRepo = Substitute.For<IRepository<Notificacion, Guid>>();
            mockNotifRepo.GetAsync(notifId).Returns(notif);

            var mockCurrentUser = Substitute.For<ICurrentUser>();

            var service = new NotificacionAppService(mockNotifRepo, mockCurrentUser);

            await service.MarcarNoLeidaAsync(notifId);

            notif.Leida.ShouldBeFalse();
            await mockNotifRepo.Received(1).UpdateAsync(notif);
        }

        // -------------------------------------------------------------
        // 4) Obtener notificaciones del usuario actual
        // -------------------------------------------------------------

        [Fact]
        public async Task GetMisNotificacionesAsync_ShouldReturnCurrentUserNotifications()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // Mock ICurrentUser
            var mockCurrentUser = Substitute.For<ICurrentUser>();
            mockCurrentUser.Id.Returns(userId);

            // Mock repositorio
            var mockNotifRepo = Substitute.For<IRepository<Notificacion, Guid>>();

            mockNotifRepo
                .GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Notificacion, bool>>>())
                .Returns(Task.FromResult(
                     new List<Notificacion>
            {
                new Notificacion(Guid.NewGuid()) { UserId = userId, Mensaje = "N1" },
                new Notificacion(Guid.NewGuid()) { UserId = userId, Mensaje = "N2" }
         }
                ));

            // Crear service
            var service = new NotificacionAppService(mockNotifRepo, mockCurrentUser);

            // ACT
            var result = await service.GetMisNotificacionesAsync();

            // ASSERT
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].Mensaje.ShouldBe("N1");
            result[1].Mensaje.ShouldBe("N2");
        }


        // -------------------------------------------------------------
        // 5) Error si no hay usuario actual
        // -------------------------------------------------------------
        [Fact]
        public async Task GetMisNotificacionesAsync_NoUser_ShouldThrow()
        {
            var mockNotifRepo = Substitute.For<IRepository<Notificacion, Guid>>();
            var mockCurrentUser = Substitute.For<ICurrentUser>();

            mockCurrentUser.Id.Returns((Guid?)null);

            var service = new NotificacionAppService(mockNotifRepo, mockCurrentUser);

            await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(() =>
                service.GetMisNotificacionesAsync());
        }
    }
}
