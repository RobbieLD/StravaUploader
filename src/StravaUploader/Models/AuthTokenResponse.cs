using System.Text.Json.Serialization;

namespace StravaUploader.Models
{
    public class AuthTokenResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType{ get; set; } = string.Empty;

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        public bool IsExpired
        {
            get
            {
                return ExpiresAt < DateTimeOffset.Now.ToUnixTimeSeconds();
            }
        }
    }
}
