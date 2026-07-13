using Fcg.Notification.Application.UseCase.RejectPaymentEmail;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface ISendPaymentRejectUseCase
    {
        Task ExecuteAsync(SendPaymentRejectCommand command, CancellationToken cancellationToken);
    }
}
