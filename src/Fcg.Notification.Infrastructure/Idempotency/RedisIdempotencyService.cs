using Fcg.Notification.Application.Ports;
using Fcg.Notification.Infrastructure.Caching;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Fcg.Notification.Infrastructure.Idempotency
{
    public class RedisIdempotencyService : IIdempotencyService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisSettings _redisOptions;

        public RedisIdempotencyService(IConnectionMultiplexer redis, IOptions<RedisSettings> redisOptions)
        {
           _redis = redis;
           _redisOptions = redisOptions.Value;
        }

        public async Task ReleaseAsync(Guid eventId)
        {
            var db = _redis.GetDatabase();
            var key = $"FiapCloudGames:idempotency:notification:on:{eventId}";
            await db.KeyDeleteAsync(key);
        }

        public async Task<bool> TryProcessAsync(Guid eventId)
        {
            var db = _redis.GetDatabase();
            var key = $"FiapCloudGames:idempotency:notification:on:{eventId}";
            var expiry = TimeSpan.FromDays(_redisOptions.ExpirationInDays);

            bool isAcquired = await db.StringSetAsync(key, "processing_or_processed", expiry, When.NotExists);

            return isAcquired;
        }
    }
}
