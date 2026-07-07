using Fcg.Notification.Application.Common.Interfaces;
using Fcg.Notification.Application.Models;
using System.Net.Http.Json;

namespace Fcg.Notification.Application.Client
{
    public class UserApiClient : IUserApiClient
    {
        private readonly HttpClient _httpClient;
        public UserApiClient(HttpClient httpClient) => _httpClient = httpClient;    
        public async Task<UserProfileCacheModel?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/api/internal/users/{userId}", cancellationToken);
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserProfileCacheModel>(cancellationToken);  
        }
    }
}
