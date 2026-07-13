namespace Fcg.Notification.Infrastructure.Caching
{
    public class RedisSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 6379;
        public string Password { get; set; } = string.Empty;
        public string InstanceName { get; set; }

        public RedisSettings() { }

        public const string RedisSectionName = "RedisSettings";
        public int ExpirationInDays { get; set; } = 3;
    }
}
