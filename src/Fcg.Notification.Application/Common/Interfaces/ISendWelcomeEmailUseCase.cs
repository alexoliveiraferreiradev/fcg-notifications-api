using Fcg.Notification.Application.UseCase.WelcomeEmail;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface ISendWelcomeEmailUseCase
    {
        Task ExecuteAsync(SendWelcomeEmailCommand command, CancellationToken cancellationToken);
    }
}
