using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;
using FluentAssertions;

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
            var notification = new Domain.Entities.Notification(emailAddress, NotificationType.Welcome);

            // Assert
            notification.Id.Should().NotBeEmpty();
            notification.Recipient.Address.Should().Be("test@fiap.com");
            notification.Type.Should().Be(NotificationType.Welcome);            
            notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            notification.SendAt.Should().BeNull();
            notification.FailureReason.Should().BeNull();
        }

        
    }
}
