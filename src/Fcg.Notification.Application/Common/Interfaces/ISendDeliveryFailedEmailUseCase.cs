using Fcg.Notification.Application.UseCase.DeliveryFailedEmail;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface ISendDeliveryFailedEmailUseCase
    {
        Task ExecuteAsync(SendDeliveryFailedEmailCommand command, CancellationToken cancellationToken);
    }
}
