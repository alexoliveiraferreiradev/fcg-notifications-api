using Fcg.Notification.Domain.Entities;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;
using FluentAssertions;
using System;
using Xunit;

namespace Fcg.Notification.Domain.Tests.Entities
{
    public class NotificationTests
    {
        [Fact]
        public void Construtor_DeveInicializarComStatusPendente_E_SemDataDeEnvio()
        {
            // Arrange
            var emailAddress = EmailAddress.Create("test@fiap.com");

            // Act
            var notification = new Fcg.Notification.Domain.Entities.Notification(emailAddress, NotificationType.Welcome);

            // Assert
            notification.Id.Should().NotBeEmpty();
            notification.Recipient.Value.Should().Be("test@fiap.com");
            notification.Type.Should().Be(NotificationType.Welcome);
            notification.Status.Should().Be(NotificationStatus.Pending);
            notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            notification.SendAt.Should().BeNull();
            notification.FailureReason.Should().BeNull();
        }

        [Fact]
        public void MarkAsSent_DeveAlterarStatusParaEnviado_E_PreencherDataDeEnvio()
        {
            // Arrange
            var emailAddress = EmailAddress.Create("test@fiap.com");
            var notification = new Fcg.Notification.Domain.Entities.Notification(emailAddress, NotificationType.OrderConfirmation);

            // Act
            notification.MarkAsSent();

            // Assert
            notification.Status.Should().Be(NotificationStatus.Sent);
            notification.SendAt.Should().NotBeNull();
            notification.SendAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            notification.FailureReason.Should().BeNull();
        }

        [Fact]
        public void MarkAsFailure_DeveAlterarStatusParaFalha_E_RegistrarMotivo()
        {
            // Arrange
            var emailAddress = EmailAddress.Create("test@fiap.com");
            var notification = new Fcg.Notification.Domain.Entities.Notification(emailAddress, NotificationType.Welcome);
            var reason = "SMTP timeout";

            // Act
            notification.MarkAsFailure(reason);

            // Assert
            notification.Status.Should().Be(NotificationStatus.Failed);
            notification.FailureReason.Should().Be(reason);
            notification.SendAt.Should().BeNull();
        }
    }
}
