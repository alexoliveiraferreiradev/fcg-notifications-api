using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Fcg.Notification.Infrastructure.Services
{
    public class UserProfileIntegrationService : IUserProfileIntegrationService
    {
        private readonly IDistributedCache _cache;

        public UserProfileIntegrationService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<UserProfileCacheModel> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user:{userId}:profile";
            var cached  = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<UserProfileCacheModel>(cached);
            else
                throw new DomainException(DomainMessages.UserNotFound);
        }
    }
}
