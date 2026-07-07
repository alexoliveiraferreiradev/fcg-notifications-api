namespace Fcg.Notification.Application.UseCase.RejectPaymentEmail
{
    public record SendPaymentRejectCommand(Guid EventId,  Guid OrderId, Guid UserId,string Reason);
}
