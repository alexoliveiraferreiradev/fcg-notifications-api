using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Ports;
using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.UseCase.DeliveryFailedEmail
{
    public class SendDeliveryFailedEmailUseCase 
    {
        private readonly IEmailService _emailService;
        private readonly IIdempotencyService _idempotencyService;
        private readonly IUserProfileIntegrationService _userProfileIntegrationService;

        public SendDeliveryFailedEmailUseCase(IEmailService emailService, IIdempotencyService idempotencyService, 
            IUserProfileIntegrationService userProfileIntegrationService)
        {
            _emailService = emailService;
            _idempotencyService = idempotencyService;
            _userProfileIntegrationService = userProfileIntegrationService;
        }

        public async Task ExecuteAsync(SendDeliveryFailedEmailCommand command, CancellationToken cancellationToken)
        {
            if (!await _idempotencyService.TryProcessAsync(command.EventId))
                return;

            try
            {
                var userProfile = await _userProfileIntegrationService.GetUserProfileAsync(command.EventId,cancellationToken);

                var emailRecipient = EmailAddress.Create(userProfile.Email);


                var notification = new Domain.Entities.Notification(emailRecipient, NotificationType.OrderConfirmation);

                var (subject, body) = notification.GenerateDeliveryFailedContent(command.OrderId, userProfile.UserName);

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
