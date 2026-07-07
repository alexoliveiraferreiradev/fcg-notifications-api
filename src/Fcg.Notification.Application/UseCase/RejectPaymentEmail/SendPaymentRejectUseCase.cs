using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.UseCase.RejectPaymentEmail
{
    public class SendPaymentRejectUseCase
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;
        private readonly IUserProfileIntegrationService _userIntegrationService;

        public SendPaymentRejectUseCase(IEmailService emailService, IIdempotencyService idempotencyService, 
            IUserProfileIntegrationService userIntegrationService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
            _userIntegrationService = userIntegrationService;
        }

        public async Task ExecuteAsync(SendPaymentRejectCommand command, CancellationToken cancellationToken)
        {   
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {
                var userProfile = await _userIntegrationService.GetUserProfileAsync(command.UserId, cancellationToken);

                var emailRecipient = EmailAddress.Create(userProfile.Email);


                var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.OrderConfirmation);

                var (subject, body) = notification.GeneratePaymentRejectionContent(command.OrderId, userProfile.UserName, command.Reason);

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
