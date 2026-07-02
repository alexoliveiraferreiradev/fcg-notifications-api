using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailAddress recipient, string subject, string body, CancellationToken cancellationToken);       
    }
}
