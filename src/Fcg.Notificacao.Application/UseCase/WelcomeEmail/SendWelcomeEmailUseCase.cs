using Fcg.Notificacao.Application.Common.Interfaces;
using Fcg.Notificacao.Application.Ports;
using Fcg.Notificacao.Domain.Entities;
using Fcg.Notificacao.Domain.Enum;
using Fcg.Notificacao.Domain.ValueObject;
using System.Threading;

namespace Fcg.Notificacao.Application.UseCase.WelcomeEmail
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
            var notification = new Notification(emailRecipient, NotificationType.Welcome);

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
