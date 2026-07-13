using Fcg.Notification.Application.UseCase.ApprovedPaymentEmail;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface ISendPaymentApprovedEmailUseCase
    {
        Task ExecuteAsync(SendPaymentApprovedEmailCommand command, CancellationToken cancellationToken);
    }
}
