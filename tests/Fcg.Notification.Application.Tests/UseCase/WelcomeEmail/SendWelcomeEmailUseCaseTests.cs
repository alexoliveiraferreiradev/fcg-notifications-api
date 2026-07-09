using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.WelcomeEmail;
using Fcg.Notification.Domain.ValueObject;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Notification.Application.Tests.UseCase.WelcomeEmail
{
    public class SendWelcomeEmailUseCaseTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IIdempotencyService> _idempotencyServiceMock;
        private readonly Mock<IDistributedCache> _cache;
        private readonly SendWelcomeEmailUseCase _useCase;
        private readonly Mock<ILogger<SendWelcomeEmailUseCase>> _loggerMock;
        public SendWelcomeEmailUseCaseTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _idempotencyServiceMock = new Mock<IIdempotencyService>();
            _cache = new Mock<IDistributedCache>();
            _loggerMock = new Mock<ILogger<SendWelcomeEmailUseCase>>();

            _useCase = new SendWelcomeEmailUseCase(
                _emailServiceMock.Object,
                _idempotencyServiceMock.Object,
                _cache.Object,
                _loggerMock.Object

            );
        }

        [Fact]
        public async Task ExecuteAsync_NaoDeveEnviarEmail_QuandoJaProcessado()
        {
            // Arrange
            var command = new SendWelcomeEmailCommand(Guid.NewGuid(), Guid.NewGuid(), "teste@teste.com", "Test");
            _idempotencyServiceMock.Setup(s => s.TryProcessAsync(command.EventId)).ReturnsAsync(false);

            // Act
            await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _idempotencyServiceMock.Verify(s => s.TryProcessAsync(It.IsAny<Guid>()), Times.Once);
            _idempotencyServiceMock.Verify(s => s.ReleaseAsync(command.EventId), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_DeveEnviarEmailEMarcarComoProcessado_QuandoCaminhoFeliz()
        {
            // Arrange
            var command = new SendWelcomeEmailCommand(Guid.NewGuid(), Guid.NewGuid(), "teste@teste.com", "Joao");
            _idempotencyServiceMock.Setup(s => s.TryProcessAsync(command.EventId)).ReturnsAsync(true);

            // Act
            await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(
                It.Is<EmailAddress>(addr => addr.Address == command.Email), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()), Times.Once);
                
            _idempotencyServiceMock.Verify(s => s.TryProcessAsync(command.EventId), Times.Once);
            _idempotencyServiceMock.Verify(s => s.ReleaseAsync(command.EventId), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_DeveLancarExcecao_QuandoFalharNoEnvio()
        {
            // Arrange
            var command = new SendWelcomeEmailCommand(Guid.NewGuid(), Guid.NewGuid(), "teste@teste.com", "Joao");
            _idempotencyServiceMock.Setup(s => s.TryProcessAsync(command.EventId)).ReturnsAsync(true);
            
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<EmailAddress>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SMTP Timeout"));

            // Act
            Func<Task> act = async () => await _useCase.ExecuteAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("SMTP Timeout");
            _idempotencyServiceMock.Verify(s => s.TryProcessAsync(It.IsAny<Guid>()), Times.Once);
            _idempotencyServiceMock.Verify(s => s.ReleaseAsync(command.EventId), Times.Once);
        }
    }
}
