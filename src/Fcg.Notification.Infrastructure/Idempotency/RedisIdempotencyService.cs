using Fcg.Notification.Application.Ports;
using Fcg.Notification.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Fcg.Notification.Infrastructure.Idempotency
{
    public class RedisIdempotencyService : IIdempotencyService
    {
        private readonly IDistributedCache _cache;
        private readonly RedisOptions _redisOptions;
        
        public RedisIdempotencyService(IDistributedCache cache, IOptions<RedisOptions> redisOptions)
        {
           _cache = cache;
           _redisOptions = redisOptions.Value;
        }
        public async Task<bool> HasBeenProcessedAsync(Guid eventId)
        {
            var cachedValue = await _cache.GetStringAsync(eventId.ToString());
            return !string.IsNullOrEmpty(cachedValue);
        }

        public async Task MarkAsProcessedAsync(Guid eventId)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_redisOptions.ExpirationInDays)
            };

            await _cache.SetStringAsync(eventId.ToString(), "processed", options);
        }
    }
}
