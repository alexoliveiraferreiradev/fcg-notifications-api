namespace Fcg.Notificacao.Domain.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(Guid UsuarioId, string Nome, string Email);
        Task SendApprovedPaymentEmail(Guid UsuarioId,Guid OrderId, string NomeUsuario, 
            string Email,DateTime DataAquisicao);
    }
}
