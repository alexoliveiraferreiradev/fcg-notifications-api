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
        private readonly IUserApiClient _userApiClient;

        public UserProfileIntegrationService(IDistributedCache cache, IUserApiClient userApiClient)
        {
            _cache = cache;
            _userApiClient = userApiClient;
        }

        public async Task<UserProfileCacheModel> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user:{userId}:profile";
            var cached  = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<UserProfileCacheModel>(cached);

            var profile = await _userApiClient.GetProfileAsync(userId, cancellationToken);
            if (profile == null) throw new DomainException(DomainMessages.UserNotFound);

            await _cache.SetStringAsync(cacheKey,JsonSerializer.Serialize(profile),cancellationToken);
            return profile; 
        }
    }
}
