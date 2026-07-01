namespace Fcg.Notificacao.Application.Ports
{
    public interface IIdempotencyService
    {
        Task<bool> HasBeenProcessedAsync(Guid eventId);
        Task MarkAsProcessedAsync(Guid eventId);
    }
}
