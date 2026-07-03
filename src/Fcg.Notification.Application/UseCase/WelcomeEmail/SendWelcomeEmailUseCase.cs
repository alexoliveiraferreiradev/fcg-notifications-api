using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

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

        public async Task ExecuteAsync(SendWelcomeEmailCommand command, CancellationToken cancellationToken)
        {
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {

            var emailRecipient = EmailAddress.Create(command.Email);
            var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.Welcome);

            var (subject, body) = notification.GenerateWelcomeContent(command.UserName);
         
            await _emailService.SendEmailAsync(notification.Recipient, subject, body, cancellationToken);

            }
            catch
            {
                await _idempotencyService.ReleaseAsync(command.EventId);

                throw;
            }
        }
    }
}
