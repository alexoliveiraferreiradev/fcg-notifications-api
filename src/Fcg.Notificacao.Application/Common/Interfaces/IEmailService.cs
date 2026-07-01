using Fcg.Notificacao.Domain.ValueObject;

namespace Fcg.Notificacao.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailAddress recipient, string subject, string body, CancellationToken cancellationToken);       
    }
}
