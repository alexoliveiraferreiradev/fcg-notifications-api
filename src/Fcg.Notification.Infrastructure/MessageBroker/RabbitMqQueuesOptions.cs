namespace Fcg.Notification.Infrastructure.MessageBroker
{
    public class RabbitMqQueuesOptions
    {
        public const string SectionName = "RabbitMqQueues";
        public string NotificationUserCreatedQueue { get; set; } = string.Empty;
        public string NotificationPaymentFailedQueue { get; set; } = string.Empty;
        public string NotificationPaymentProcessedQueue { get; set; } = string.Empty;
        public string NotificationDeliveryFailedQueue { get; set; } = string.Empty;

    }
}
