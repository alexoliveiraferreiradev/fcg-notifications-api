using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Entities;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.UseCase.ApprovedPaymentEmail
{
    public class SendPaymentApprovedEmailUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;

        public SendPaymentApprovedEmailUseCase(IEmailService emailService, IIdempotencyService idempotencyService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
        }

        public async Task ExecuteAsync(SendPaymentApprovedEmailCommand command, CancellationToken cancellationToken)
        {
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {

            var emailRecipient = EmailAddress.Create(command.Email);


            var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.OrderConfirmation);

            var (subject, body) = notification.GenerateOrderConfirmationContent(command.OrderId, command.UserName,command.CreatedAt);

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
