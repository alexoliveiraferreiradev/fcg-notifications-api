namespace Fcg.Notificacao.Infrastructure.Caching
{
    public class RedisOptions
    {
        public string Configuration { get; set; }
        public string InstanceName { get; set; }
        public int ExpirationInDays { get; set; } = 3;
    }
}
