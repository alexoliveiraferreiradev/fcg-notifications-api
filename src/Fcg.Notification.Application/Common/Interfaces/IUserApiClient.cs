using Fcg.Notification.Application.Models;

namespace Fcg.Notification.Application.Common.Interfaces
{
    public interface IUserApiClient
    {
        Task<UserProfileCacheModel?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
