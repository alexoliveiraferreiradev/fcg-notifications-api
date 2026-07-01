namespace Fcg.Notificacao.Application.UseCase.ApprovedPaymentEmail
{
    public record SendPaymentApprovedEmailCommand(Guid EventId,Guid UsuarioId, Guid OrderId, string NomeUsuario, string Email, DateTime DataAquisicao);
}
