using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Domain.ValueObject;
using Microsoft.Extensions.Logging;

namespace Fcg.Notification.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }       
        public Task SendEmailAsync(EmailAddress recipient, string subject, string body, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=== SIMULANDO ENVIO DE E-MAIL ===");
            _logger.LogInformation("To: {Recipient}", recipient.Address);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);
            _logger.LogInformation("=================================");

            return Task.CompletedTask;
        }

    
    }
}
