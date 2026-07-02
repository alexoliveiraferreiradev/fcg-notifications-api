namespace Fcg.Notification.Application.UseCase.WelcomeEmail
{
    public record SendWelcomeEmailCommand(Guid EventId, Guid UserId, string Email, string UserName);
}
