namespace Fcg.Notification.Application.UseCase.RejectPaymentEmail
{
    public record SendPaymentRejectCommand(Guid EventId,  Guid OrderId, string UserName, string Email, string Reason);
}
