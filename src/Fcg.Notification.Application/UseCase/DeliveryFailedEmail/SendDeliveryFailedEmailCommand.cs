namespace Fcg.Notification.Application.UseCase.DeliveryFailedEmail
{
    public record SendDeliveryFailedEmailCommand(Guid EventId, Guid OrderId, Guid UserId, string Reason);
}
