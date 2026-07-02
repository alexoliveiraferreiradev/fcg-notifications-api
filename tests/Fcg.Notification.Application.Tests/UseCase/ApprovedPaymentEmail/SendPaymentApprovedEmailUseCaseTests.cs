using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using Fcg.Notification.Domain.ValueObject;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Notification.Application.Tests.UseCase.ApprovedPaymentEmail
{
    public class SendPaymentApprovedEmailUseCaseTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IIdempotencyService> _idempotencyServiceMock;
        private readonly SendPaymentApprovedEmailUseCase _useCase;

        public SendPaymentApprovedEmailUseCaseTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _idempotencyServiceMock = new Mock<IIdempotencyService>();

            _useCase = new SendPaymentApprovedEmailUseCase(
                _emailServiceMock.Object,
                _idempotencyServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_NaoDeveEnviarEmail_QuandoJaProcessado()
        {
            // Arrange
            var command = new SendPaymentApprovedEmailCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste", "teste@teste.com", DateTime.UtcNow);
            _idempotencyServiceMock.Setup(s => s.HasBeenProcessedAsync(command.EventId)).ReturnsAsync(true);

            // Act
            await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _idempotencyServiceMock.Verify(s => s.MarkAsProcessedAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_DeveEnviarEmailEMarcarComoProcessado_QuandoCaminhoFeliz()
        {
            // Arrange
            var command = new SendPaymentApprovedEmailCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste", "teste@teste.com", DateTime.UtcNow);
            _idempotencyServiceMock.Setup(s => s.HasBeenProcessedAsync(command.EventId)).ReturnsAsync(false);

            // Act
            await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(
                It.Is<EmailAddress>(addr => addr.Value == command.Email), 
                It.Is<string>(s => s.Contains(command.OrderId.ToString())), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
                
            _idempotencyServiceMock.Verify(s => s.MarkAsProcessedAsync(command.EventId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_DeveLancarExcecao_QuandoFalharNoEnvio()
        {
            // Arrange
            var command = new SendPaymentApprovedEmailCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste", "teste@teste.com", DateTime.UtcNow);
            _idempotencyServiceMock.Setup(s => s.HasBeenProcessedAsync(command.EventId)).ReturnsAsync(false);
            
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SMTP Timeout"));

            // Act
            Func<Task> act = async () => await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("SMTP Timeout");
            _idempotencyServiceMock.Verify(s => s.MarkAsProcessedAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
