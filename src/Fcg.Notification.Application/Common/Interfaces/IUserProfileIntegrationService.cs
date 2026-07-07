using Fcg.Notification.Application.Models;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface IUserProfileIntegrationService
    {
        Task<UserProfileCacheModel> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
