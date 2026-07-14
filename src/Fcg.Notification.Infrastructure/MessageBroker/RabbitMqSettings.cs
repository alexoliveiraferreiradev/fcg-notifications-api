using System.ComponentModel.DataAnnotations;

namespace Fcg.Notification.Infrastructure.MessageBroker
{
    public class RabbitMqSettings
    {
        [Required(ErrorMessage = "O Host do RabbitMQ é obrigatório.")]
        public string Host { get; set; } = string.Empty;
        [Range(1, 65535, ErrorMessage = "A porta do RabbitMQ deve ser válida (1 a 65535).")]
        public ushort Port { get; set; } = 5672;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public const string SectionName = "RabbitMqSettings";
        [Required]
        public string NotificationUserCreatedQueue { get; set; } = string.Empty;
        [Required]
        public string NotificationPaymentFailedQueue { get; set; } = string.Empty;
        [Required]
        public string NotificationPaymentProcessedQueue { get; set; } = string.Empty;
        [Required]
        public string NotificationDeliveryFailedQueue { get; set; } = string.Empty;

    }
}
