namespace Fcg.Notification.Application.UseCase.ApprovedPaymentEmail
{
    public record SendPaymentApprovedEmailCommand(Guid EventId,Guid UsuarioId, Guid OrderId, string UserName, string Email, DateTime CreatedAt);
}
