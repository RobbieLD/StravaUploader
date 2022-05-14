
using StravaUploader.Models;

namespace StravaUploader
{
    public interface IKeyRepository
    {
        Task<AuthTokenResponse> GetAsync();
        Task SetAsync(AuthTokenResponse auth);
        bool Exists();
    }
}