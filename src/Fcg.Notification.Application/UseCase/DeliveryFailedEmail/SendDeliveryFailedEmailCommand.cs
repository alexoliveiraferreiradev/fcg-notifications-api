namespace Fcg.Notification.Application.UseCase.DeliveryFailedEmail
{
    public record SendDeliveryFailedEmailCommand(Guid EventId, Guid OrderId, string UserName, string Email, string Reason);
}
