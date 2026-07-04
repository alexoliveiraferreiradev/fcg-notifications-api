using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.UseCase.RejectPaymentEmail
{
    public class SendPaymentRejectUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;

        public SendPaymentRejectUseCase(IEmailService emailService, IIdempotencyService idempotencyService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
        }

        public async Task ExecuteAsync(SendPaymentRejectCommand command, CancellationToken cancellationToken)
        {   
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {

                var emailRecipient = EmailAddress.Create(command.Email);


                var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.OrderConfirmation);

                var (subject, body) = notification.GeneratePaymentRejectionContent(command.OrderId, command.UserName, command.Reason);

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
