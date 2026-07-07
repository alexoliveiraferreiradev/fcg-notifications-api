namespace Fcg.Notification.Application.UseCase.ApprovedPaymentEmail
{
    public record SendPaymentApprovedEmailCommand(Guid EventId,Guid UsuarioId, Guid OrderId, DateTime CreatedAt);
}
