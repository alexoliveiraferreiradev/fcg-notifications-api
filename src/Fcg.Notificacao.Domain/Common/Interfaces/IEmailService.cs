namespace Fcg.Notificacao.Domain.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(Guid UsuarioId, string Nome, string Email);
    }
}
