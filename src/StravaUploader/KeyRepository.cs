using StravaUploader.Models;
using System.Text.Json;

namespace StravaUploader
{
    public class KeyRepository : IKeyRepository
    {
        private const string fileName = "auth.json";

        public async Task<AuthTokenResponse> GetAsync()
        {
            using FileStream stream = File.OpenRead(fileName);
            return await JsonSerializer.DeserializeAsync<AuthTokenResponse>(stream) ?? throw new EndOfStreamException("auth file not in correct format");
        }

        public async Task SetAsync(AuthTokenResponse auth)
        {
            using FileStream stream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(stream, auth);
            await stream.DisposeAsync();
        }

        public bool Exists()
        {
            return File.Exists(fileName);
        }
    }
}
