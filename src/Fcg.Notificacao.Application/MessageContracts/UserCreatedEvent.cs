namespace Fcg.MessageContracts
{
    public record UserCreatedEvent(
        Guid UserId,
        string Nome,
        string Email);
}
