using Fcg.Notificacao.Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fcg.Notificacao.Infrastructure.Services
{
    public class FakeEmailService : IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;
        public FakeEmailService(ILogger<FakeEmailService> logger)
        {
            _logger = logger;
        }
        public async Task SendWelcomeEmailAsync(Guid UsuarioId, string Nome, string Email)
        {            
            await Task.Delay(500);

            _logger.LogInformation("==================================================");
            _logger.LogInformation("📩 [NOTIFICATIONS API] - DISPARO DE E-MAIL INICIADO");
            _logger.LogInformation("==================================================");
            _logger.LogInformation("Para:      {Email}", Email);
            _logger.LogInformation("Assunto:   Bem-vindo(a) ao sistema FCG, {Nome}!", Nome);
            _logger.LogInformation("Corpo:     Olá {Nome}, sua conta foi criada com sucesso.", Nome);
            _logger.LogInformation("           Guarde o seu ID de registro: {UserId}", UsuarioId);
            _logger.LogInformation("==================================================");
            _logger.LogInformation("✅ STATUS: E-mail enviado (Simulado) com sucesso!");
            _logger.LogInformation("==================================================");
        }
    }
}
