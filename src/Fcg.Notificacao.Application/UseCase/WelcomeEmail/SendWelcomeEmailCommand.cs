namespace Fcg.Notificacao.Application.UseCase.WelcomeEmail
{
    public record SendWelcomeEmailCommand(Guid EventId, Guid UserId, string Email, string UserName);
}
