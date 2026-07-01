using Fcg.Notificacao.Domain.Enum;
using Fcg.Notificacao.Domain.ValueObject;

namespace Fcg.Notificacao.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public EmailAddress Recipient { get; private set; }
        public NotificationType Type { get; private set; }
        public NotificationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? SendAt { get; private set; }
        public string? FailureReason { get; private set; }

        public Notification(EmailAddress recipient, NotificationType type)
        {
            Id = Guid.NewGuid();
            Recipient = recipient;
            Type = type;
            Status = NotificationStatus.Pending;
            CreatedAt = DateTime.UtcNow;   
        }

        public void MarkAsSent()
        {
            if (Status == NotificationStatus.Sent)
                return;

            Status = NotificationStatus.Sent;
            SendAt = DateTime.UtcNow;
            FailureReason = null;
        }

        public void MarkAsFailure(string reason)
        {
            Status = NotificationStatus.Failed;
            FailureReason = reason;
        }


    }
}
