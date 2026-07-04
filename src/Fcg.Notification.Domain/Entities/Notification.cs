using Fcg.Notification.Domain.Enum;
using Fcg.Notification.Domain.ValueObject;

namespace Fcg.Notification.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public EmailAddress Recipient { get; private set; }
        public NotificationType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? SendAt { get; private set; }
        public string? FailureReason { get; private set; }

        public Notification(EmailAddress recipient, NotificationType type)
        {
            Id = Guid.NewGuid();
            Recipient = recipient;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        public (string Subject, string Body) GenerateWelcomeContent(string userName)
        {
            return (
            "Bem-vindo à FIAP Cloud Games!",
            $"Olá {userName}, a sua conta foi criada com sucesso."
            );
        }
        public (string Subject, string Body) GenerateOrderConfirmationContent(Guid orderId, string userName, DateTime date)
        {
            return (
                $"Pagamento aprovado com sucesso para o pedido {orderId}!",
                $"Olá {userName}, o seu pagamento foi aprovado com sucesso. Data Aquisição: {date}"
            );
        }

        public (string Subject, string Body) GeneratePaymentRejectionContent(Guid orderId, string userName, string reason)
        {
            return (
                $"Pagamento rejeitado para o pedido {orderId}.",
                $"Olá {userName}, infelizmente o seu pagamento foi rejeitado. Motivo: {reason}"
            );
        }


        public (string Subject, string Body) GenerateDeliveryFailedContent(Guid orderId, string userName)
        {
            return (
                $"Falha na entrega do pedido {orderId}.",
                $"Olá {userName}, infelizmente houve uma falha na entrega do seu pedido. O estorno já foi solicitado "
            );
        }
    }
}
