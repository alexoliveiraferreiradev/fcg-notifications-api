using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Entities;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;
using System.Threading;

namespace Fcg.Notification.Application.UseCase.WelcomeEmail
{
    public class SendWelcomeEmailUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;

        public SendWelcomeEmailUseCase(IEmailService emailService, IIdempotencyService idempotencyService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
        }

        public async Task ExecuteAsync(SendWelcomeEmailCommand command,CancellationToken cancellationToken)
        {
            if (await _idempotencyService.HasBeenProcessedAsync(command.EventId))
                return;


            var emailRecipient = EmailAddress.Create(command.Email);
            var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.Welcome);

            try
            {
                var subject = "Bem-vindo à FIAP Cloud Games!";
                var body = $"Olá {command.UserName}, a sua conta foi criada com sucesso.";

                await _emailService.SendEmailAsync(notification.Recipient, subject, body, cancellationToken);

                notification.MarkAsSent();

                await _idempotencyService.MarkAsProcessedAsync(command.EventId);
            }
            catch (Exception ex)
            {
                notification.MarkAsFailure(ex.Message);
                throw;
            }
        }
    }
}
